package pg.spi.authenticator;

import org.keycloak.authentication.AuthenticationFlowContext;
import org.keycloak.authentication.Authenticator;
import org.keycloak.authentication.AuthenticationFlowError;
import org.keycloak.credential.CredentialInput;
import org.keycloak.models.KeycloakSession;
import org.keycloak.models.RealmModel;
import org.keycloak.models.UserCredentialModel;
import org.keycloak.models.UserModel;
import org.jboss.logging.Logger;

public class PhoneAuthenticator implements Authenticator{
    private static final Logger log = Logger.getLogger(PhoneAuthenticator.class);

    @Override
    public void authenticate(AuthenticationFlowContext context) {
        // Show login form (FTL) that has phoneNumber and password fields
        log.debug("Entering authenticate() for realm " + context.getRealm().getName());
        context.challenge(context.form().createForm("login-phone.ftl"));
    }

    @Override
    public void action(AuthenticationFlowContext context) {
        String phone = context.getHttpRequest()
            .getDecodedFormParameters().getFirst("phone");
        String pwd   = context.getHttpRequest()
            .getDecodedFormParameters().getFirst("password");

        log.infof("Attempting login for phone number %s", phone);

        CredentialInput input = UserCredentialModel.password(pwd);
        
        UserModel user = context.getSession().users()
            .searchForUserByUserAttributeStream(context.getRealm(), "phone", phone).findFirst().orElse(null);

        if (user == null) {
            log.warnf("No user found with phone %s", phone);
            context.failureChallenge(AuthenticationFlowError.INVALID_USER,
            context.form().setError("Invalid phone number").createForm("login-phone.ftl"));
            return;
        }

        if (user.credentialManager().isValid(input)) {
            log.infof("Authentication success for user %s", user.getUsername());
            context.setUser(user);
            context.success();
        }
        else {
            context.failureChallenge(AuthenticationFlowError.INVALID_CREDENTIALS,
            context.form().setError("Invalid credentials").createForm("login-phone.ftl"));
        }
    }

    // Boilerplate (no-op)
    @Override public boolean requiresUser() { return false; }
    @Override public void close() {}

    @Override
    public boolean configuredFor(KeycloakSession session, RealmModel realm, UserModel user) {
        return true;
    }

    @Override
    public void setRequiredActions(KeycloakSession session, RealmModel realm, UserModel user) {
        return;
    }
}
