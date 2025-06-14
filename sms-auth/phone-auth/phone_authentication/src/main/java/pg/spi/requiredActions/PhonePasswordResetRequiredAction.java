package pg.spi.requiredActions;

import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;

import org.jboss.logging.Logger;
import org.keycloak.authentication.RequiredActionContext;
import org.keycloak.authentication.RequiredActionProvider;
import org.keycloak.authentication.actiontoken.resetcred.ResetCredentialsActionToken;
import org.keycloak.common.util.Time;
import org.keycloak.models.KeycloakSession;
import org.keycloak.models.UserModel;

import com.twilio.Twilio;
import com.twilio.rest.api.v2010.account.Message;
import com.twilio.type.PhoneNumber;

public class PhonePasswordResetRequiredAction implements RequiredActionProvider{
    private static final Logger log = Logger.getLogger(PhonePasswordResetRequiredAction.class);
    private final KeycloakSession session;

    public PhonePasswordResetRequiredAction(KeycloakSession session) {
        this.session = session;
    }

    @Override
    public void evaluateTriggers(RequiredActionContext context) {
        // Implement reset trigger logic based on login attempts
    }

    @Override
    public void requiredActionChallenge(RequiredActionContext context) {
        UserModel user = context.getUser();
        String phoneNumber = user.getFirstAttribute("phone");

        log.info("Phone " + phoneNumber);
        
        // Parameters needed:
        String userId = user.getId();
        String email = user.getEmail();
        int expirationInSecs = Time.currentTime() + 3600; // 1 hour expiration
        String authSessionId = context.getAuthenticationSession().getTabId();
        String clientId = context.getAuthenticationSession().getClient().getClientId();

        // Create the token instance
        ResetCredentialsActionToken resetToken = new ResetCredentialsActionToken(
            userId, 
            email, 
            expirationInSecs, 
            authSessionId, 
            clientId
        );

        // Serialize token to string
        String tokenString = resetToken.serialize(session, context.getRealm(), context.getUriInfo());
        log.info("Token " + tokenString);

        String ACCOUNT_SID = "-";
        String AUTH_TOKEN = "-";
        Twilio.init(ACCOUNT_SID, AUTH_TOKEN);

        String resetUrl = "http://localhost:18080/realms/master/login-actions/action-token?key=" + URLEncoder.encode(tokenString, StandardCharsets.UTF_8);
        String messageBody = "To reset your password, please visit: " + resetUrl;
        
        System.out.println(messageBody);

        // Send token to your endpoint
        try {
            String endpointUrl = "http://blazorfurniture:8080/api/Shorten/shorten";
            java.net.URL url = new java.net.URL(endpointUrl);
            java.net.HttpURLConnection conn = (java.net.HttpURLConnection) url.openConnection();
            conn.setRequestMethod("POST");
            conn.setRequestProperty("Content-Type", "application/json");
            conn.setDoOutput(true);

            String jsonPayload = "{\"url\": \"" + resetUrl + "\"}";
            try (java.io.OutputStream os = conn.getOutputStream()) {
                byte[] input = jsonPayload.getBytes(StandardCharsets.UTF_8);
                os.write(input, 0, input.length);
            }

            int responseCode = conn.getResponseCode();
            log.info("POST Response Code :: " + responseCode);

            // Optionally, read the response
            try (java.io.BufferedReader br = new java.io.BufferedReader(
                    new java.io.InputStreamReader(conn.getInputStream(), StandardCharsets.UTF_8))) {
                StringBuilder response = new StringBuilder();
                String responseLine;
                while ((responseLine = br.readLine()) != null) {
                    response.append(responseLine.trim());
                }
                log.info("Response: " + response.toString());

                String shortUrl = response.toString().split("\"shortUrl\":\"")[1].split("\"")[0];

                Message message = Message
                .creator(
                    new PhoneNumber("+18777804236"),
                    new PhoneNumber("+18162392534"),
                    shortUrl
                )
                .create();
                System.out.println(message.getSid());
            }
            conn.disconnect();
        } catch (Exception e) {
            log.error("Failed to send token to endpoint", e);
        }

        context.challenge(context.form().createForm("phone-reset-sent.ftl"));
    }

    @Override
    public void close() {
    }

    @Override
    public void processAction(RequiredActionContext context) {
    }
}
