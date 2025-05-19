<#import "template.ftl" as layout>
<@layout.registrationLayout; section>
    <#if section = "header">
        ${msg("smsAuthTitle")}
    <#elseif section = "form">
        <form action="${url.loginAction}" method="post">
            <div class="${properties.kcFormGroupClass!}">
                <div class="${properties.kcLabelWrapperClass!}">
                    <label class="${properties.kcLabelClass!}">${msg("smsCode")}</label>
                </div>
                <div class="${properties.kcInputWrapperClass!}">
                    <input type="text" name="code" autocomplete="off" />
                </div>
            </div>
            <div class="${properties.kcFormGroupClass!}">
                <input type="submit" value="${msg("doSubmit")}" />
            </div>
        </form>
    </#if>
</@layout.registrationLayout>
