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




        


        Message message = Message
        .creator(
            new PhoneNumber("+18777804236"),
            new PhoneNumber("+18162392534"),
            messageBody
        )
        .create();
        System.out.println(message.getSid());

        message = Message
        .creator(
            new PhoneNumber("+18777804236"),
            new PhoneNumber("+18162392534"),
            "Test"
        )
        .create();

        context.challenge(context.form().createForm("phone-reset-sent.ftl"));
    }

    @Override
    public void close() {
    }

    @Override
    public void processAction(RequiredActionContext context) {
    }
}
