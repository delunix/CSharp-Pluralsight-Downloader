(function () {
    'use strict';

    angular.module('app')
    .controller('courseCtrl', courseCtrl);

    courseCtrl.$inject = ['$rootScope', 'coursesService', '_', 'Hub', '$timeout', 'toaster'];

    function courseCtrl($rootScope, coursesService, _, Hub, $timeout, toaster) {
        var vm = this;
        vm.courseName = '';
        vm.loadCourseData = loadCourseData;
        vm.course = undefined;
        vm.downloadClip = downloadClip;

        activate();
        /////////////////////////////////////

        function activate() {
            //declaring the hub connection
            var hub = new Hub('ProgressHub', {
                //client side methods
                listeners: {
                    'updateProgress': function (progress) {
                        var requiredClip = _.get(vm.course, '.courseModules[' + progress.extra.moduleIndex + ']'
                            + '.clips[' + progress.extra.clipIndex + ']');

                        $timeout(function () {
                            if (requiredClip) {
                                requiredClip.progress = progress;
                            }
                        }, 0);
                    }
                }
            });
        }

        function loadCourseData() {
            var title = vm.courseName;
            if (vm.courseName.indexOf('/') > 0) {
                title = _.last(vm.courseName.split('/'));
            }
            return coursesService.getCourseData(title).then(function (course) {
                if (course.title == null) {
                    alert('Course not found!.');
                    vm.course = undefined;
                } else {
                    vm.course = course;
                }
            });
        }

        function downloadClip(clip, module) {
            clip.progress.isDownloading = true;
            toaster.pop({
                type: 'info',
                title: '',
                body: 'Starting saving video <b>"' + clip.title + '</b>".',
                bodyOutputType: 'trustedHtml'
            });
            clip.courseTitle = vm.course.title;
            clip.moduleTitle = module.title;
            clip.moduleIndex = _.findIndex(vm.course.courseModules, function (moduleItem) {
                return moduleItem.title == module.title;
            });
            return coursesService.downloadCourseModuleClip(clip).then(function (progress) {
                toaster.pop({
                    type: 'success',
                    title: '',
                    body: 'Video file <b>"' + progress.fileName + '</b>" saved.',
                    bodyOutputType: 'trustedHtml'
                });
                $timeout(function () {
                    clip.progress.hasBeenDownloaded = true;
                    clip.progress.isDownloading = false;
                }, 0);
            }, function (errorResponse) {
                debugger;
                switch (errorResponse.status) {
                    case 429:
                        toaster.pop({
                            type: 'error',
                            title: '',
                            body: 'Couldn\'t download video <b>"' + clip.title + '</b> due to many requests in short time". Trying again in 5 seconds.',
                            bodyOutputType: 'trustedHtml',
                            timeout: '5000',
                            progressBar: true,
                            onHideCallback: function () {
                                vm.downloadClip(clip, module, true);
                            }
                        });
                        break;
                    case 422:
                        toaster.pop({
                            type: 'error',
                            title: '',
                            body: 'Invalid user name or password.',
                        });
                        break;
                    default:
                        toaster.pop({
                            type: 'error',
                            title: '',
                            body: 'Couldn\'t download video <b>"' + clip.title + '</b>". Error: ' + errorResponse.error,
                            bodyOutputType: 'trustedHtml'
                        });
                        break;
                }
                clip.progress.isDownloading = false;
                clip.progress.hasBeenDownloaded = false;
            });
        }
    }
})();