angular.module("umbraco").controller("PdfExport.DashboardController", function ($scope, $http, editorState) {
    $scope.nodeId = editorState.current && editorState.current.id;
    $scope.nodeName = editorState.current && editorState.current.name;
    $scope.isGenerating = false;
    $scope.generationSuccess = false;
    $scope.generationError = null;
    $scope.pdfUrl = null;
    $scope.pdfSize = null;

    $scope.generatePdf = function () {
        if (!$scope.nodeId) return;
        $scope.isGenerating = true;
        $scope.generationSuccess = false;
        $scope.generationError = null;
        $scope.pdfUrl = null;
        $scope.pdfSize = null;

        $http({
            method: "GET",
            url: "/umbraco/backoffice/PdfExport/PdfExportDashboard/GeneratePdf",
            params: { contentId: $scope.nodeId },
            responseType: "arraybuffer"
        }).then(function (response) {
            var blob = new Blob([response.data], { type: "application/pdf" });
            $scope.pdfUrl = URL.createObjectURL(blob);
            $scope.pdfSize = blob.size;
            $scope.generationSuccess = true;
        }, function (error) {
            $scope.generationError = "Failed to generate PDF";
        }).finally(function () {
            $scope.isGenerating = false;
        });
    };

 

    // Export all nodes
    $scope.exportAllNodes = function () {
        window.location.href = "/umbraco/backoffice/PdfExport/PdfExportDashboard/ExportAllNodes";
    };

    // Get node name
    $scope.getNodeName = function (id) {
        return $http.get("/umbraco/backoffice/PdfExport/PdfExportDashboard/GetNodeName", {
            params: { id: id }
        }).then(function (response) {
            return response.data;
        });
    };

    // Initialize
    $scope.loadContentTree();
}); 