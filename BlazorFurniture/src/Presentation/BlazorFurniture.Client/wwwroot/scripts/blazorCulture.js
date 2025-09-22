window.blazorCulture = {
    set: function (value) {
        document.cookie = "BlazorCulture=" + value + "; path=/";
        localStorage.setItem('BlazorCulture', value);
    },
    get: function () {
        return localStorage.getItem('BlazorCulture');
    }
};
