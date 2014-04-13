$(document).ready(function () {
    LayoutScript.initialize();
});

var LayoutScript = new function () {
    var self = {
        initialize: function () {
            self.initializeAnimations();

            AnimationManager.start();

            var doorShutterConfig = new DoorShutterBase.getConfigObject();
            doorShutterConfig.BoxColor = "#BCC4F5";
            var doorShutterTargets = $(".doorShutter");
            for (var i = 0; i < doorShutterTargets.length; i++) {
                var target = doorShutterTargets[i];
                var testScript = new DoorShutter(target, doorShutterConfig);
            }

            arrangeNavLinks();
        },
        initializeAnimations: function () {
            var logoAnimation = new LogoAnimation($(".logo-div"));

            AnimationManager.registerAnimation(logoAnimation);
        }
    };

    function arrangeNavLinks() {
        var $container = $("#menuContainer");
        var $navDivs = $container.find(".navDiv");
        var containerWidth = $container.width();
        var $offsetContainer = $container.find(".menuOffsetContainer");

        var spacing = 10;
        var navDivWidth = 0;
        $navDivs.each(function (index, navDiv) {
            var $navDiv = $(navDiv);

            navDivWidth += $navDiv.width() + spacing;
        });

        var offset = 0;
        $navDivs.each(function (index, navDiv) {
            var $navDiv = $(navDiv);

            $navDiv.css("position", "absolute");
            $navDiv.css("left", offset);
            offset += $navDiv.width() + spacing;
        });

        $offsetContainer.css("position", "relative");
        $offsetContainer.css("margin-left", (containerWidth - navDivWidth) / 2.0);
    }

    return self;
}();

//This prototype function allows you to remove even array from array
Array.prototype.remove = function(x) { 
    for(i in this){
        if(this[i].toString() == x.toString()){
            this.splice(i, 1);
        }
    }
}

var AnimationManager = new function () {
    var timer = null;
    var lastUpdate = null;
    var animations = [];
    //FIXME: Implement animation tracking
    //var activeAnimations
    //var counter = 1;

    function getTicks() {
        var currentDateTime = new Date();
        return currentDateTime.getTime();
    }

    var self = {
        start: function () {
            timer = setInterval(self.update, (1 / 60) * 1000);
            lastUpdate = getTicks();
        },
        setActive: function(animationObject){
            //TODO
        },
        setInactive: function(animationObject){
            //TODO
        },
        registerAnimation: function(animationObject){
            animations.push(animationObject);
        },
        unregisterAnimation: function(animationObject){
            animations.remove(animationObject);
        },
        update: function () {
            window.clearInterval(timer);

            var currentUpdate = getTicks();
            var delta = (currentUpdate - lastUpdate)/1000.0;
            for (var i = 0; i < animations.length; i++) {
                var animation = animations[i];
                animation.update(delta);
            }

            lastUpdate = currentUpdate;
            timer = setInterval(self.update, (1 / 100) * 1000);
        }
    };

    return self;
}();

var LogoAnimation = function (targetDiv) {
    var targetDiv = targetDiv;
    var tracker = 0.0;
    var endingTracker = 0.0;
    var imgNormal = targetDiv.find(".imgNormal");
    var imgExcited = targetDiv.find(".imgExcited");
    var isActive = false;
    var isEnding = false;
    var borderWidth = 0.0;
    var borderWidthChangeRate = 35.0;
    var borderWidthMax = 5.0;
    var rotationRate = 50.0; // degs/sec
    var rotationDone = 0.0;
    var endingTotalTime = 0.15;
    var rotationScalar = 40.0;
    var isIntro = true;
    var maxIntroTime = 1.0;

    function toggleAnimation() {
        if (!isActive) {
            startAnimation();
        }
        else {
            if (isEnding) {
                reset();
                startAnimation();
            }
            else {
                stopAnimation();
            }
        }
    }

    function startAnimation() {
        isActive = true;
        isIntro = false;
        isEnding = false;
        tracker = 0.0;
        borderWidth = 0.0;
        imgNormal.css("display", "none");
        imgExcited.css("display", "inline");
    }

    function stopAnimation() {
        isEnding = true;
        endingTracker = tracker;
    }

    function reset() {
        isActive = false;
        isEnding = false;
        isIntro = false;
        tracker = 0.0;
        endingTracker = 0.0;
        GlobalUtilities.SetCssBorder(targetDiv, 0.0);
        imgNormal.css("display", "inline");
        imgExcited.css("display", "none");
        rotationDone = false;
    }

    function setRotation(item, magnitude) {
        var rotationString = "rotate(" + parseInt(magnitude) + "deg)";
        item.css("transform", rotationString);
    }

    function simplePulseFunction(cyclePercentage) {
        return (1.0 + Math.sin((cyclePercentage * Math.PI * 2.0) - Math.PI / 2.0)) / 2.0;
    }

    reset();
    isIntro = true;

    targetDiv.mouseenter(function () {
        if (GlobalUtilities.isMobile()) {
            return;
        }
        startAnimation();
    });
    targetDiv.mouseleave(function () {
        if (GlobalUtilities.isMobile()) {
            return;
        }
        stopAnimation();
    });
    targetDiv.click(toggleAnimation);

    var self = {
        update: function (delta) {
            if (isIntro) {
                tracker = tracker + delta;
                var borderWidth = 6.0 * simplePulseFunction(2.0* tracker / maxIntroTime);
                GlobalUtilities.SetCssBorder(targetDiv, borderWidth);

                if (tracker > maxIntroTime) {
                    isIntro = false;
                    GlobalUtilities.SetCssBorder(targetDiv, 0.0);
                }
                return;
            }

            if (!isActive) {
                return;
            }

            if (!isEnding) {
                tracker = tracker + delta;
                var rotation = Math.sin(tracker * 10.0);
                setRotation(imgExcited, rotationScalar * rotation);

                var borderWidth = tracker * borderWidthChangeRate;
                borderWidth = (borderWidth > borderWidthMax) ? borderWidthMax : borderWidth;
                GlobalUtilities.SetCssBorder(targetDiv, borderWidth);
            }
            else {
                endingTracker += delta;
                if (endingTracker - tracker > endingTotalTime) {
                    reset();
                }
                else {
                    var percentage = (endingTracker - tracker) / endingTotalTime;
                    var rotation = Math.sin(tracker * 10.0) * (1.0 - percentage);
                    setRotation(imgExcited, rotationScalar * rotation);
                    var borderWidth = (1.0-percentage) * borderWidthMax;
                    GlobalUtilities.SetCssBorder(targetDiv, borderWidth);
                }

            }
        }
    };

    return self;
}