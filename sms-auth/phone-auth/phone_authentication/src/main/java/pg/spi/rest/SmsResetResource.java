package pg.spi.rest;

import org.jboss.logging.Logger;
import org.keycloak.authentication.actiontoken.DefaultActionToken;
import org.keycloak.authentication.actiontoken.resetcred.ResetCredentialsActionToken;
import org.keycloak.models.*;
import org.keycloak.urls.UrlType;

import com.fasterxml.jackson.databind.JsonNode;
import com.google.rpc.Status;

import jakarta.ws.rs.*;
import jakarta.ws.rs.core.*;
import java.util.Collections;

@Path("/realms/{realm}/sms-reset")
public class SmsResetResource {
        private static final Logger log = Logger.getLogger(SmsResetResource.class);

    @Context
    private KeycloakSession session;

    public SmsResetResource(KeycloakSession keycloakSession) {
        session = keycloakSession;
    }

    @POST
    @Produces(MediaType.APPLICATION_JSON)
    public Response generateSmsReset(
        @PathParam("realm") String realmName,
        JsonNode body) {
        
        log.info("Body " + realmName);
        
        String userId = body.get("userId").asText();
        String email  = body.get("email").asText();
        String clientId = body.get("clientId").asText();
        String redirectUri = body.get("redirectUri").asText();

        log.info("Body " + body);

        // 1. Lookup realm & user
        RealmModel realm = session.realms().getRealmByName(realmName);
        KeycloakContext context = session.getContext();

        log.info("Realm is " + realm);
        ResetCredentialsActionToken token = new ResetCredentialsActionToken(
            userId,
            email,
            600,
            null,
            clientId
        );
        
        log.info("Token is " + token);
        // 3. Serialize the token into a compact string
        UriInfo uri;
        
        if (context.getUri(UrlType.FRONTEND) != null) {
            uri = context.getUri(UrlType.FRONTEND);
            log.info("Frontend uri " + uri);
        }
        else
        {
            uri = context.getUri(UrlType.BACKEND);
            log.info("Backend uri " + uri);
        }

        if (realm == null) {
            log.info("Realm is null " );
            return Response.status(jakarta.ws.rs.core.Response.Status.BAD_REQUEST).build();
        }

        log.info("Realm id " + realm.getId());

        String serialized = token.serialize(session, realm, uri);

        // Build the reset link
        String keycloakBase = session.getContext().getUri().getBaseUriBuilder()
            .path("realms").path(realm.getName())
            .path("login-actions").path("reset-credentials")
            .queryParam("key", token)
            .queryParam("client_id",clientId)
            .queryParam("redirect_uri",redirectUri)
            .build(serialized)
            .toString();

        log.info("Keycloase base " + keycloakBase);

        return Response.ok(Collections.singletonMap("link", keycloakBase)).build();
    }
}
