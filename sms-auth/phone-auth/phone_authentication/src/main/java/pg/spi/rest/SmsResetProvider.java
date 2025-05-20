package pg.spi.rest;

import org.keycloak.models.KeycloakSession;
import org.keycloak.services.resource.RealmResourceProvider;

public class SmsResetProvider implements RealmResourceProvider {

    private final KeycloakSession session;

    public SmsResetProvider(KeycloakSession session) {
        this.session = session;
    }

    @Override
    public Object getResource() {
        return new SmsResetResource(session);
    }

    @Override
    public void close() {
        // Cleanup resources if necessary
    }
}
