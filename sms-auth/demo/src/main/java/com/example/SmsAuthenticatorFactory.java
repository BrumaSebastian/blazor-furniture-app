package com.example;

import java.util.List;

import org.keycloak.Config.Scope;
import org.keycloak.authentication.Authenticator;
import org.keycloak.authentication.AuthenticatorFactory;
import org.keycloak.models.KeycloakSession;
import org.keycloak.models.KeycloakSessionFactory;
import org.keycloak.provider.ProviderConfigProperty;
import org.keycloak.models.AuthenticationExecutionModel;
import org.keycloak.models.AuthenticationExecutionModel.Requirement;

public class SmsAuthenticatorFactory implements AuthenticatorFactory {
    public static final String PROVIDER_ID = "sms-authenticator";

    @Override
    public Authenticator create(KeycloakSession session) {
        return new SmsAuthenticator();
    }

    @Override
    public String getId() {
        return PROVIDER_ID;
    }

    @Override
    public String getDisplayType() {
        return "SMS Authentication";
    }

    @Override
    public void init(Scope config) {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'init'");
    }

    @Override
    public void postInit(KeycloakSessionFactory factory) {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'postInit'");
    }

    @Override
    public void close() {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'close'");
    }

    @Override
    public String getReferenceCategory() {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'getReferenceCategory'");
    }

    @Override
    public boolean isConfigurable() {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'isConfigurable'");
    }

    @Override
    public Requirement[] getRequirementChoices() {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'getRequirementChoices'");
    }

    @Override
    public boolean isUserSetupAllowed() {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'isUserSetupAllowed'");
    }

    @Override
    public String getHelpText() {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'getHelpText'");
    }

    @Override
    public List<ProviderConfigProperty> getConfigProperties() {
        // TODO Auto-generated method stub
        throw new UnsupportedOperationException("Unimplemented method 'getConfigProperties'");
    }
    
    // Implement remaining required methods
}