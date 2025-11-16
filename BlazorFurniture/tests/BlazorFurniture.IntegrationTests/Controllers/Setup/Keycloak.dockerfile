FROM quay.io/keycloak/keycloak:26.4.0

# Copy your custom SPI JARs
COPY ./keycloak-spis-all-1.0.0.jar /opt/keycloak/providers/

# Copy the exported realm JSON
COPY ./main-realm-export.json /opt/keycloak/data/import/main-realm.json

# Build Keycloak with the custom providers
RUN /opt/keycloak/bin/kc.sh build