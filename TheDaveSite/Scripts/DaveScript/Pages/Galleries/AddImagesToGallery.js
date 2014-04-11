$(document).ready(function () {
    AddImagesToGalleryScript.initialize();
});

var AddImagesToGalleryScript = new function () {
    var filesList = [];
    var errorsList = [];
    var filesProcessed = 0;
    var filesUploaded = 0;

    var LoadedImage = function () {
        var self = {
            FileName: "",
            FileUrl: "",
            FileReference: null
        };

        return self;
    }

    function CancelEvent(e) {
        if (e.preventDefault) { e.preventDefault(); }
        return false;
    }

    function FilesComplete() {
        $("#invisible_image_holder").html("");
        $("#image_output").html("");


        $.each(filesList, function (index, item) {
            AddLocalThumbnail(item.FileUrl);
        });

        //if (GlobalUtilities.isInternetExplorer()) {
        //    $("#invisible_image_holder img").bind("load", function () {
        //        var resizedImage = ImageResizer.getResizedImage(this, 100, 100);
        //        $("#image_output").append(resizedImage);
        //    });
        //}
        //else {
        //    $("#invisible_image_holder").waitForImages(function (a) {
        //        $("#invisible_image_holder img").each(function (index, item) {
        //            var resizedImage = ImageResizer.getResizedImage(item, 100, 100);
        //            $("#image_output").append(resizedImage);
        //        });
        //    });
        //}
    }

    function AddLocalThumbnail(fileUrl) {
        //var sourceImage = new Image();
        //sourceImage.src = fileUrl;
        //$("#invisible_image_holder").append(sourceImage);

        if (GlobalUtilities.isInternetExplorer()) {
            //$("#invisible_image_holder img").bind("load", function () {
            //    var resizedImage = ImageResizer.getResizedImage(this, 100, 100);
            //    $("#image_output").append(resizedImage);
            //    $("#invisible_image_holder").empty();
            //});

            //var resizedImage = ImageResizer.getResizedImage(this, 100, 100);
            //$("#image_output").append(resizedImage);
            //$("#invisible_image_holder").empty();

            $("#invisible_image_holder").waitForImages(function (a) {
                $("#invisible_image_holder img").each(function (index, item) {
                    var resizedImage = ImageResizer.getResizedImage(item, 100, 100);
                    $("#image_output").append(resizedImage);
                    $("#invisible_image_holder").empty();
                });
            });
        }
        else {
            $("#invisible_image_holder").waitForImages(function (a) {
                $("#invisible_image_holder img").each(function (index, item) {
                    var resizedImage = ImageResizer.getResizedImage(item, 100, 100);
                    $("#image_output").append(resizedImage);
                    $("#invisible_image_holder").empty();
                });
            });
        }

       $("#invisible_image_holder").empty();
    }

    function ClearErrors() {
        errorsList = [];
        $("#error_messages").css("display", "none");
        $("#error_messages ul").empty();
    }

    function AddError(errorMessage) {
        $("#error_messages ul").append("<li>" + errorMessage + "</ul>");
    }

    function HandleDrop(e) {
        CancelEvent(e);

        var fileSource = e.originalEvent.dataTransfer || e.target.files;
        var files = fileSource.files;

        ClearErrors();

        filesProcessed = 0;

        for (var i = 0; i < files.length; i++) {
            var file = files[i];

            if (!validateErrorConditions(file)) {
                continue;
            }

            SetFileReadCallback(file, files.length);
        }

        if (errorsList.length > 0)
        {
            $("#error_messages").css("display", "inline");

            $.each(errorsList, function (index, item) {
                AddError(item);
            });
        }
    }

    function validateErrorConditions(file) {
        var matchArray = GlobalUtilities.arrayMatch(filesList, function (item) {
            return item.FileName === file.name;
        });
        if (matchArray.length > 0) {
            errorsList.push("" + file.name + ": Image has already been added.");
            return false;
        }

        if (file.type !== "image/jpeg" && file.type !== "image/gif" && file.type !== "image/png") {
            errorsList.push("" + file.name + ": " + file.type + " is not a supported file type.");
            return false;
        }

        if (file.size > self.MAX_FILE_SIZE) {
            errorsList.push("" + file.name + ": File is larger than maximum size of " + self.MAX_FILE_SIZE_READABLE + ".");
            return false;
        }

        return true;
    }

    function SetFileReadCallback(file, fileCount) {
        var filename = file.name;
        var reader = new FileReader();

        reader.onloadend = function (e) {
            var newImage = new LoadedImage();

            newImage.FileName = filename;
            newImage.FileUrl = e.target.result;
            newImage.FileReference = file;

            filesList.push(newImage);

            filesProcessed++;
            if (filesProcessed === fileCount) {
                FilesComplete();
            }
        };

        reader.readAsDataURL(file);
    }

    function UploadFiles() {
        GlobalScript.GlobalSpinner.show();

        if (filesList.length > 0) {
            UploadNextFile(0);
        }
    }

    function DragDropHover() {
        $("#dragdroptarget").addClass("dragdroptarget_active");
        $("#dragdroptarget").removeClass("dragdroptarget_passive");
    }

    function DragDropDehover() {
        $("#dragdroptarget").addClass("dragdroptarget_passive");
        $("#dragdroptarget").removeClass("dragdroptarget_active");
    }

    function UploadNextFile(fileIndex) {
        var form = $("#uploadForm");
        var formData = new FormData(form[0]);

        $("#progress_updater").html("Uploading file #" + (fileIndex + 1) + " of " + filesList.length);

        formData.append("images", filesList[fileIndex].FileReference);

        if (formData) {
            $.ajax({
                url: form[0].action,
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: function (res) {
                    fileIndex = fileIndex + 1;
                    if (fileIndex === filesList.length) {
                        GlobalScript.GlobalSpinner.hide();
                        alert("Upload successful!");
                        $("#progress_updater").html("Done!");
                        window.location = "/Galleries/Galleries";
                    }
                    else {
                        UploadNextFile(fileIndex);
                    }
                },
                error: function (x, y, z) {
                    alert("Encountered error.");
                }
            });
        }
    }

    var self = {
        initialize: function () {
            var xhr = new XMLHttpRequest();
            if (xhr.upload) {
                $("#dragdroptarget_actual").on("drop", HandleDrop);
                $("#dragdroptarget_actual").on("dragover", function (e) {
                    CancelEvent(e);
                });
                $("#dragdroptarget_actual").on("dragenter", function (e) {
                    DragDropHover();
                    CancelEvent(e);
                });
                $("#dragdroptarget_actual").on("dragleave", function (e) {
                    DragDropDehover();
                    CancelEvent(e);
                });

                $("#btn_submit_images").click(function (e) {
                    UploadFiles();
                });
            }
        },
        MAX_FILE_SIZE: 6000000,
        MAX_FILE_SIZE_READABLE: "6mb"
    };

    return self;
}();