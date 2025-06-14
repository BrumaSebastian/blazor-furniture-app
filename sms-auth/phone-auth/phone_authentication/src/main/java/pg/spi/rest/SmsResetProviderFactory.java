package pg.spi.rest;

import org.keycloak.Config;
import org.keycloak.models.KeycloakSession;
import org.keycloak.models.KeycloakSessionFactory;
import org.keycloak.services.resource.RealmResourceProvider;
import org.keycloak.services.resource.RealmResourceProviderFactory;

public class SmsResetProviderFactory implements RealmResourceProviderFactory {

    public static final String ID = "sms-reset";

    @Override
    public RealmResourceProvider create(KeycloakSession session) {
        return new SmsResetProvider(session);
    }

    @Override
    public void init(Config.Scope config) {
        // Initialization logic if needed
    }

    @Override
    public void postInit(KeycloakSessionFactory factory) {
        // Post-initialization logic if needed
    }

    @Override
    public void close() {
        // Cleanup logic if needed
    }

    @Override
    public String getId() {
        return ID;
    }
}
