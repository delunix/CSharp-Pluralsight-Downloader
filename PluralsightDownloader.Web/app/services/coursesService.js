(function () {
    'use strict';

    angular.module('app')
    .factory('coursesService', coursesService);

    coursesService.$inject = ['$http', '$q', 'appCache', '_', 'cfg'];

    function coursesService($http, $q, appCache, _, cfg) {
        var self = this;
        self.coursesCache = 'plsi_courses';

        var service = {
            getCourseData: getCourseData,
            downloadCourseModuleClip: downloadCourseModuleClip
        };

        return service;

        /////////////////////////////////////

        function getCourseData(courseName) {
            var deferred = $q.defer();

            var cachedCourses = appCache.get(self.coursesCache) || [];
            var requiredCourse = _.filter(cachedCourses, function (course) {
                return course.name === courseName;
            });
            if (requiredCourse[0]) {
                deferred.resolve(requiredCourse[0]);
            }
            else {
                $http.get('/api/courses/' + courseName)
                .success(function (data, status, headers, config) {
                    cachedCourses.push(data);
                    appCache.put(self.coursesCache, cachedCourses);
                    deferred.resolve(data);
                })
                .error(function (error, status, headers, config) {
                    deferred.reject({ error: error.exceptionMessage, status: status });
                });
            }

            return deferred.promise;
        }

        function downloadCourseModuleClip(clip) {
            var deferred = $q.defer();
            $http.post('/api/courses/clip/' + clip.name + '/download', clip)
            .success(function (data, status, headers, config) {
                deferred.resolve(data);
            })
            .error(function (error, status, headers, config) {
                deferred.reject({ error: error.exceptionMessage, status: status });
            });

            return deferred.promise;
        }
    }
})();