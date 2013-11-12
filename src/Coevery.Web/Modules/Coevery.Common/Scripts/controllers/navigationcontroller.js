﻿define(['core/app/detourService', 'core/services/navigationdataservice'], function(detour) {
    detour.registerController([
        'NavigationCtrl',
        ['$scope', '$state', '$stateParams', 'navigationDataService', 'logger',
            function($scope, $state, $stateParams, navigationDataService, logger) {
                var navigationId = $state.params.NavigationId;

                $scope.currentMenu = {};
                $scope.menus = {};

                $scope.init = function() {
                    var menus = navigationDataService.query(function () {
                        
                        var selectedMenu = $.grep(menus, function (n) {
                            return n.IdHint == navigationId;
                        });

                        if (selectedMenu.length > 0)
                            $scope.currentMenu = selectedMenu[0];
                        else
                            $scope.currentMenu = (menus || []).length > 0 ? menus[0] : {};

                        $.each($scope.currentMenu.Items || [], function (i, n) {
                            n.class = (n.Classes || []).join(" ");
                        });
                        
                        $scope.menus = menus;

                    }, function() {
                        logger.error("Failed to fetched menus in front");
                    });
                };

                $scope.setcurrmenu = function (id) {
                    $.each($scope.menus,function(i, item) {
                        if (item.IdHint == id){
                            $scope.currentMenu = item;
                            return;
                        }
                    });
                    
                    $.each($scope.currentMenu.Items || [], function (i, item) {
                        item.class = (item.Classes || []).join(" ");
                    });
                };

                $scope.init();
            }]
    ]);
});