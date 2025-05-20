package pg.spi.requiredActions;

import org.keycloak.Config.Scope;
import org.keycloak.authentication.RequiredActionFactory;
import org.keycloak.authentication.RequiredActionProvider;
import org.keycloak.models.KeycloakSession;
import org.keycloak.models.KeycloakSessionFactory;

public class PhonePasswordResetRequiredActionFactory implements RequiredActionFactory {
    @Override
    public RequiredActionProvider create(KeycloakSession session) {
        return new PhonePasswordResetRequiredAction(session);
    }

    @Override
    public String getId() {
        return "phone_password_reset";
    }

    @Override
    public void init(Scope config) {
    }

    @Override
    public void postInit(KeycloakSessionFactory factory) {
    }

    @Override
    public void close() {
    }

    @Override
    public String getDisplayText() {
        return "Sms-reset-password";
    }
}
