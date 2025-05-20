package pg.spi.requiredActions;

import java.time.LocalDate;

import org.jboss.logging.Logger;
import org.keycloak.authentication.RequiredActionContext;
import org.keycloak.authentication.RequiredActionProvider;
import org.keycloak.authentication.actiontoken.resetcred.ResetCredentialsActionToken;
import org.keycloak.common.util.Time;
import org.keycloak.models.KeycloakSession;
import org.keycloak.models.UserModel;

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

        // // Send SMS
        // SmsService smsService = session.getProvider(SmsService.class);
        // smsService.sendResetSms(phoneNumber, resetToken);
        
        context.challenge(context.form().createForm("phone-reset-sent.ftl"));
    }

    @Override
    public void close() {
    }

    @Override
    public void processAction(RequiredActionContext context) {
    }
}
