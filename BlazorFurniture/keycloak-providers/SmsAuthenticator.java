public class SmsAuthenticator implements Authenticator {

    @Override
    public void authenticate(AuthenticationFlowContext context) {
        UserModel user = context.getUser();
        String phoneNumber = user.getFirstAttribute("phone");

        if (phoneNumber == null || phoneNumber.isEmpty()) {
            context.challenge(context.form().setError("Missing phone number").createForm("sms-error.ftl"));
            return;
        }

        String code = generateOtp();
        context.getSession().getAttributeStore().put("sms_code", code);

        sendSms(phoneNumber, code);

        context.challenge(context.form().createForm("sms-otp.ftl"));
    }

    @Override
    public void action(AuthenticationFlowContext context) {
        String submittedCode = context.getHttpRequest().getDecodedFormParameters().getFirst("otp");
        String actualCode = (String) context.getSession().getAttributeStore().get("sms_code");

        if (submittedCode != null && submittedCode.equals(actualCode)) {
            context.success();
        } else {
            context.failureChallenge(AuthenticationFlowError.INVALID_CREDENTIALS,
                context.form().setError("Invalid OTP").createForm("sms-otp.ftl"));
        }
    }

    // Other required methods...
    // close, requiresUser, setRequiredActions etc.

    private String generateOtp() {
        return String.format("%06d", new Random().nextInt(999999));
    }

    private void sendSms(String phoneNumber, String code) {
        // Integrate Java SMS library here (e.g., Twilio)
        System.out.println("Send OTP " + code + " to phone " + phoneNumber);
    }
}
