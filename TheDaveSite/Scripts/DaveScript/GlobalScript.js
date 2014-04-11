$(document).ready(function () {
    GlobalScript.GlobalInitialize();
});

var GlobalScript = new function () {
    var self = {
        GlobalSpinner: null,
        IsAuthenticated: false,
        GlobalInitialize: function () {
            self.GlobalSpinner = new Spinner("#globalSpinner");

            $("label[aria-required]").each(function () {
                $(this).before($("<span class='fieldRequired'>*</span>"));
            });
        },
        GetAppUrl: function(){
            return $("#pageUrl").val();
        },
        JsPropertyCopyIfAvailable: function (full, partial) {
            if (partial === undefined) {
                return;
            }

            for (var property in full) {
                if (partial[property] !== undefined) {
                    full[property] = partial[property];
                }
            }
        }
    };

    return self;
}();


var GlobalUtilities = new function () {
    var self = {
        SetCssBorder: function (item, width) {
            var borderString = width + "px";
            item.css("borderTopWidth", borderString);
            item.css("borderBottomWidth", borderString);
            item.css("borderLeftWidth", borderString);
            item.css("borderRightWidth", borderString);
        },
        isMobile: function () {
            var index = navigator.appVersion.indexOf("Mobile");
            return (index > -1);
        },
        isInternetExplorer: function(){
            return (navigator.userAgent.indexOf('MSIE') !== -1 || navigator.appVersion.indexOf('Trident/') > 0);
        },
        roundUp: function (number) {

        },
        arrayMatch: function (someArray, tester) {
            var testedArray = [];

            $.each(someArray, function (index, item) {
                if (tester(item)) {
                    testedArray.push(item);
                }
            });

            return testedArray;
        },
        arrayPrune: function (someArray, tester) {
            var newArray = [];

            $.each(someArray, function (index, item) {
                if (!tester(item)) {
                    newArray.push(item);
                }
            });

            return newArray;
        },
        splitWithEmptyRemove: function (someString, separator) {
            var splitArray = someString.split(separator);
            var returnArray = [];
            for (var i = 0; i < splitArray.length; i++) {
                var entry = splitArray[i];
                if (entry.trim() !== "") {
                    returnArray.push(entry);
                }
            }

            return returnArray;
        },
        getElementDataObject: function(domElement){
            if (domElement.davedata === null || domElement.davedata === undefined) {
                domElement.davedata = {};
            }
            return domElement.davedata;
        },
        setTransition: function (target, transitionProperties, time, transitionType)
        {
            var valuesString = "";
            for (var i = 0; i < transitionProperties.length; i++) {
                var property = transitionProperties[i];

                if (i > 0) {
                    valuesString += ", "
                }
                valuesString += property + " " + time + "s " + transitionType;
            }

            target.css("-webkit-transition", valuesString);
            target.css("-moz-transition", valuesString);
            target.css("-o-transition", valuesString);
            target.css("transition", valuesString);
        },
        disableTransition: function (target, transitionProperties, values) {
            target.css("-webkit-transition", "none");
            target.css("-moz-transition", "none");
            target.css("-o-transition", "none");
            target.css("transition", "none");
        },
        buildFormDataObject: function (form) {
            var $form = $(form);
            var jsonObject = {};
            var inputs = $form.find("input, textarea, textbox");
            for (var i = 0; i < inputs.length; i++) {
                var $input = $(inputs[i]);
                jsonObject[$input.attr("id")] = $input.val();
            }

            return jsonObject;
        },
    };
    return self;
}();

var ImageResizer = new function () {
    var self = {
        getResizedImage: function (sourceImage, maxWidth, maxHeight) {
            var canvas = document.createElement('canvas');
            var context = canvas.getContext('2d');

            var thumbRatio = maxWidth / maxHeight;
            var sourceRatio = sourceImage.width / sourceImage.height;

            var targetWidth = 0, targetHeight = 0;

            if (sourceRatio > thumbRatio) {
                targetWidth = maxWidth;
                targetHeight = maxHeight / sourceRatio;
            }
            else {
                targetHeight = maxHeight;
                targetWidth = maxWidth * sourceRatio;
            }

            canvas.width = targetWidth;
            canvas.height = targetHeight;

            context.fillRect(0, 0, targetWidth, targetHeight);

            context.drawImage(
              sourceImage, 0, 0, targetWidth, targetHeight);

            var returnImage = new Image();
            returnImage.src = canvas.toDataURL();

            return returnImage;
        }
    };

    return self;
}();

var Toggler = function (one, two) {
    var self = {
        showFirst: function () {
            state = true;
            $one.css("display", "block");
            $two.css("display", "none");
        },
        showSecond: function () {
            state = false;
            $one.css("display", "none");
            $two.css("display", "block");
        },
        toggle: function () {
            state = !state;
            if (state) {
                $one.css("display", "block");
                $two.css("display", "none");
            }
            else {
                state = false;
                $one.css("display", "none");
                $two.css("display", "block");
            }
        }
    };

    var $one = $(one);
    var $two = $(two);

    // true -> first element, false = second element
    var state = true;

    self.showFirst();

    return self;
};