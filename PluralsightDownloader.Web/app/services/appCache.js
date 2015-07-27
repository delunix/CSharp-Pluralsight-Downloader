'use strict';

angular.module('app')
.factory('appCache', ['$cacheFactory', function ($cacheFactory) {
    return $cacheFactory('appCache');
}
]);