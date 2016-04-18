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
        vm.downloadModuleClips = downloadModuleClips;

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
            var title = vm.courseName.replace(/\s/g, '');
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
            }, function (error) {
                toaster.pop({
                    type: 'error',
                    title: '',
                    body: 'Couldn\'t retrieve course data. Please make sure that you have the correct course name and try again.'
                });
            });
        }

        function downloadClip(clip, module, continueDownloadNextClip) {
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
                return moduleItem.title === module.title;
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

                // should continue downloading the next clips ?
                if (continueDownloadNextClip) {
                    // if yest, // check if there is "next" clip regarding the "current" module
                    var nextClipModule = _.get(vm.course, '.courseModules[' + progress.extra.moduleIndex + ']');
                    var nextClipToDownload = nextClipModule.clips[++progress.extra.clipIndex];
                    if (nextClipToDownload) {
                        $timeout(function () {
                            return vm.downloadClip(nextClipToDownload, nextClipModule, continueDownloadNextClip);
                        }, '3000'); // 3 seconds delay to avoid blocking account.
                    }
                }
            }, function (errorResponse) {
                switch (errorResponse.status) {
                    case 429:
                        var downloadDelaySec = _.random(5, 15);
                        toaster.pop({
                            type: 'error',
                            title: '',
                            body: 'Couldn\'t download video <b>"' + clip.title + '</b> due to many requests in short time". Trying again in ' + downloadDelaySec + ' seconds.',
                            bodyOutputType: 'trustedHtml',
                            timeout: downloadDelaySec + '000',
                            progressBar: true,
                            onHideCallback: function () {
                                vm.downloadClip(clip, module, continueDownloadNextClip);
                            }
                        });
                        break;
                    case 422:
                        toaster.pop({
                            type: 'error',
                            title: '',
                            body: 'Invalid user name or password.'
                        });
                        break;
                    default:
                        toaster.pop({
                            type: 'error',
                            title: '',
                            body: 'Couldn\'t download video <b>"' + clip.title + '</b>" due to the following error: "<i>' + errorResponse.error + '</i>"',
                            bodyOutputType: 'trustedHtml'
                        });
                        break;
                }
                clip.progress.isDownloading = false;
                clip.progress.hasBeenDownloaded = false;
            });
        }

        function downloadModuleClips(module, $event) {
            if ($event) {
                $event.preventDefault();
                $event.stopPropagation();
            }
            return vm.downloadClip(module.clips[0], module, true);
        }
    }
})();