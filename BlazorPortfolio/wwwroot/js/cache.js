window.portfolioCache = {
    get: function (key) {
        try {
            return localStorage.getItem(key);
        } catch { return null; }
    },
    set: function (key, value) {
        try {
            localStorage.setItem(key, value);
        } catch { }
    },
    remove: function (key) {
        try {
            localStorage.removeItem(key);
        } catch { }
    }
};
