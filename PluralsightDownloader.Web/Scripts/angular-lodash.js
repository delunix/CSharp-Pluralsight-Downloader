var lodash = angular.module('lodash', []);
lodash.factory('_', ['$window', function ($window) {
    return $window._; // assumes lodash has already been loaded on the page
}]);