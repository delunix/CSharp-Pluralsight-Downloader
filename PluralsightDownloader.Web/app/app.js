(function () {
    'use strict';

    angular
        .module('app', ['blockUI', 'ui.bootstrap', 'ngSanitize', 'lodash', 'SignalR', 'toaster', 'ngAnimate'])
        .config(configure)
        .run(runBlock)
        .constant('cfg', {
            PLURALSIGHT: {
                BASE_URL: 'https://www.pluralsight.com/',
                LOGIN_URL: 'https://app.pluralsight.com/id/',
                COURSE_DATA_URL: 'http://www.pluralsight.com/data/course/',
                COURSE_MODULES_DATA_URL: 'http://www.pluralsight.com/data/course/content/'
            }
        });

    configure.$inject = ['$httpProvider', '$locationProvider', 'blockUIConfig'];
    runBlock.$inject = ['$rootScope'];

    function configure($httpProvider, $locationProvider, blockUIConfig) {
        $locationProvider.html5Mode(true);

        // Tell the blockUI service to ignore certain requests
        blockUIConfig.requestFilter = function (config) {
            // If the request starts with '/api/quote' ...
            if (config.url.match(/^\/api\/courses\/clip($|\/).*/)) {
                return false; // ... don't block it.
            }
        };

        //======================================
        // Routes
        //======================================
    }

    function runBlock($rootScope) {
    };
})();