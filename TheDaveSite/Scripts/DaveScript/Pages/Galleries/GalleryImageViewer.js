
var GalleryData = function () {
    var GalleryImage = function () {
        return {
            ImageIndex: -1
        };
    }

    var images = [];

    var self = {
        populateData: function (target) {
            images = [];

            var $target = $(target);
            var imageElements = $target.find("img");

            for (var i = 0; i < imageElements.length; i++) {
                var newImage = new GalleryImage();
                newImage.ImageIndex = parseInt($(imageElements[i]).attr("data-id"));

                images.push(newImage);
            }
        },
        getSize: function () {
            return images.length;
        },
        getImageList: function () {
            return images;
        },
        getIndexForImage: function (imageId) {
            for (var i = 0; i < images.length; i++) {
                if (images[i].ImageIndex === imageId) {
                    return i;
                }
            }

            return -1;
        },
        getImageForIndex: function (index) {
            return images[index];
        },
    };

    return self;
};

var Pager = function (numberOfPages, initialPage, pagesToCache) {
    if (numberOfPages === undefined) {
        throw Exception("Must supply numberOfPages.");
    }
    if (!parseInt(numberOfPages)) {
        throw Exception("Invalid value supplied for numberOfPages.");
    }
    var numberOfPages = parseInt(numberOfPages);
    if (numberOfPages < 1) {
        throw Exception("Invalid value supplied for numberOfPages.");
    }
    var pagesToCache = pagesToCache || 3;
    var currentPage = 1;

    if (initialPage !== undefined) {
        if (initialPage > numberOfPages) {
            throw Exception("initialPage must be less than or equal to numberOfPages.");
        }
        currentPage = initialPage;
    }

    listeners = [];

    function buildPageSet(currentPage) {
        var pages = [];

        var pageCounter = 1;
        var pageIndex = currentPage - 1;
        while (pageCounter <= pagesToCache && pageIndex >= 1) {
            pages.push(pageIndex);
            pageCounter++;
            pageIndex--;
        }

        pages.push(currentPage);

        pageCounter = 1;
        pageIndex = currentPage + 1;
        while (pageCounter <= pagesToCache && pageIndex <= numberOfPages) {
            pages.push(pageIndex);
            pageCounter++;
            pageIndex++;
        }

        return pages;
    }

    function buildPageDiff(oldSet, newSet) {
        var removedPages = [];
        var addedPages = [];

        for (var i = 0; i < oldSet.length; i++) {
            var oldSetItem = oldSet[i];
            if (newSet.indexOf(oldSetItem) === -1) {
                removedPages.push(oldSetItem);
            }
        }

        for (var i = 0; i < newSet.length; i++) {
            var newSetItem = newSet[i];
            if (oldSet.indexOf(newSetItem) === -1) {
                addedPages.push(newSetItem);
            }
        }

        return {
            RemovedPages: removedPages,
            AddedPages: addedPages
        };
    }

    function notifyListeners(removedPages, addedPages) {
        for (var i = 0; i < listeners.length; i++) {
            var listener = listeners[i];
            listener.pagesChanged(removedPages, addedPages);
        }
    }

    var self = {
        // Subscriber must implement function: pagesChanged(removedPages[], addedPages[])
        registerForEvents: function (listener) {
            listeners.push(listener);
        },
        getCurrentPage: function () {
            return currentPage;
        },
        isLastPage: function(){
            return currentPage === numberOfPages;
        },
        isFirstPage: function(){
            return currentPage <= 1;
        },
        getCurrentPageSet: function () {
            return buildPageSet(currentPage);
        },
        nextPage: function () {
            if (currentPage + 1 > numberOfPages) {
                // already at end, do nothing
                return;
            }

            var oldPageSet = buildPageSet(currentPage);
            currentPage++;
            var newPageSet = buildPageSet(currentPage);
            var pageDiff = buildPageDiff(oldPageSet, newPageSet);

            notifyListeners(pageDiff.RemovedPages, pageDiff.AddedPages);
        },
        getNextPage: function () {
            if (self.isLastPage()) {
                return currentPage;
            }
            return currentPage + 1;
        },
        getPreviousPage: function () {
            if (self.isFirstPage()) {
                return currentPage;
            }
            return currentPage - 1;
        },
        previousPage: function () {
            if (currentPage - 1 < 1) {
                // already at beginning, do nothing
                return;
            }

            var oldPageSet = buildPageSet(currentPage);
            currentPage--;
            var newPageSet = buildPageSet(currentPage);
            var pageDiff = buildPageDiff(oldPageSet, newPageSet);

            notifyListeners(pageDiff.RemovedPages, pageDiff.AddedPages);
        },
        destroy: function () {
            listeners = [];
        }
    };

    return self;
};

var GalleryImageViewer = new function () {
    var initialized = false;

    function CachedPageEntry() {
        var self = {
            PageContent: null,
            ImageId: -1,
            PageNumber: -1,
            CacheDiv: null
        };

        return self;
    }

    var TRANSITION_STATE = {
        NONE: 0,
        PREVIOUS_PAGE: 1,
        NEXT_PAGE: 2
    };

    var _viewer = null;
    var _viewerPreviousPage = null;
    var _viewerNextPage = null;
    var _offscreenDiv = null;
    var _cachedPages = [];
    var _pager = null;
    var _galleryData = null;
    var _currentCachePage = null;
    var _timeoutCounter = 0;
    var _timeoutHandle = null;
    var _processingNavigationEvents = false;
    var _lastPage = -1;
    var _currentTransitionState = TRANSITION_STATE.NONE;

    var TRANSITION_TIME = 0.6;
    var TIMEOUT_MAX = 10000;
    var TIMEOUT_POLL_INTERVAL = 250;

    function initialize() {
        _viewer = $("<div class='daveImageViewer'>");
        _offscreenDiv = $("<div class='daveImageViewerOffscreenDiv'>");

        $("html body").append(_viewer);
        $("html body").append(_offscreenDiv);

        initialized = true;
    }

    function getCachedPage(pageNumber) {
        var searchArray = GlobalUtilities.arrayMatch(_cachedPages, function (item) {
            return (item.PageNumber === pageNumber);
        });

        if (searchArray.length === 0) {
            return null;
        }

        return searchArray[0];
    }

    function clearCache() {
        _cachedPages = [];
        _offscreenDiv.empty();
    }

    function updatePageContent(pageNumber) {
        GlobalScript.GlobalSpinner.show();
        _timeoutCounter = 0;
        updatePageContentRecursive(pageNumber);
    }

    function updatePageContentRecursive(pageNumber) {
        var cachedPage = getCachedPage(pageNumber);
        if (null === cachedPage) /// do something???)
        {
            if (_timeoutCounter > TIMEOUT_MAX) {
                GlobalScript.GlobalSpinner.clear();
                alert("Operation timed out!");
                _processingNavigationEvents = true;
                _currentTransitionState = TRANSITION_STATE.NONE;
                return;
            }

            _timeoutHandle = window.setTimeout(function () {
                updatePageContentRecursive(pageNumber);
            }, TIMEOUT_POLL_INTERVAL);
            _timeoutCounter += TIMEOUT_POLL_INTERVAL;
        }
        else {
            _timeoutHandle = null;

            GlobalScript.GlobalSpinner.clear();

            var contentTarget = _viewer.find(".galleryImagePanel_ContentPanel");

            if (_currentTransitionState === TRANSITION_STATE.NONE) {
                cachedPage.CacheDiv.appendTo(contentTarget);
                _processingNavigationEvents = true;
                _currentCachePage = cachedPage;
                return;
            }

            cachedPage.CacheDiv.appendTo(contentTarget);
            prepTransitionProperties(cachedPage.CacheDiv);

            // Why is there a timeout here? Because Chrome and Firefox screw this up otherwise. 
            window.setTimeout(function () {
                temporaryDelay(cachedPage);
            }, 50);
        }
    }

    function temporaryDelay(cachedPage) {
        setTransitionProperties(_currentCachePage.CacheDiv, cachedPage.CacheDiv);

        window.setTimeout(function () {
            finishTransition(cachedPage);
        }, TRANSITION_TIME * 1000);
    }

    function finishTransition(newCachePage) {
        _currentCachePage.CacheDiv.removeClass("mover");
        newCachePage.CacheDiv.removeClass("mover");

        _currentCachePage.CacheDiv.appendTo(_offscreenDiv);
        _currentCachePage = newCachePage;

        if (_currentTransitionState === TRANSITION_STATE.PREVIOUS_PAGE) {
            _pager.previousPage();
        }
        else {
            _pager.nextPage();
        }

        _currentTransitionState = TRANSITION_STATE.NONE;

        self.updateViewerPanelControls();

        _processingNavigationEvents = true;
    }

    function prepTransitionProperties(newContent){
        if (_currentTransitionState === TRANSITION_STATE.PREVIOUS_PAGE){
            newContent.css("left", "-110%");
            newContent.css("right", "110%");
        }
        else{
            newContent.css("left", "110%");
            newContent.css("right", "-110%");
        }
    }

    function setTransitionProperties(oldContent, newContent) {
        newContent.addClass("mover");
        oldContent.addClass("mover");

        newContent.css("left", "10px");
        newContent.css("right", "10px");

        if (_currentTransitionState === TRANSITION_STATE.PREVIOUS_PAGE) {
            oldContent.css("left", "110%");
            oldContent.css("right", "-110%");
        }
        else {
            oldContent.css("left", "-110%");
            oldContent.css("right", "110%");
        }
    }

    function updateContent(content) {
        var contentPanel = _viewer.find(".galleryImagePanel_ContentInnerPanel");
        contentPanel.empty();
        contentPanel.append(content);
    }

    function cachePage(pageNumber) {
        var imageId = _galleryData.getImageForIndex(pageNumber - 1).ImageIndex;
        $.ajax({
            url: "/Galleries/GalleryImageContentView/?id=" + imageId,
            type: "get",
            success: function (data, textStatus, jqXHR) {
                var newCachedPage = new CachedPageEntry();

                newCachedPage.ImageId = imageId,
                newCachedPage.PageContent = data;
                newCachedPage.PageNumber = pageNumber;
                newCachedPage.CacheDiv = $(data).appendTo(_offscreenDiv);

                GalleryImageViewScript.initialize(newCachedPage.CacheDiv);

                _cachedPages.push(newCachedPage);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var x = 42;
            }
        });
    }

    function removeCachePage(pageNumber) {
        var cacheEntry = getCachedPage(pageNumber);
        cacheEntry.CacheDiv.remove();

        _cachedPages = GlobalUtilities.arrayPrune(_cachedPages, function (item) {
            return (item.PageNumber === pageNumber);
        });
    }

    function buildInitialCache() {
        var pages = _pager.getCurrentPageSet();

        for (var i = 0; i < pages.length; i++) {
            var page = pages[i];
            cachePage(page);
        }
    }
    
    function reset() {
        _viewer.empty();

        clearCache();

        _currentTransitionState = TRANSITION_STATE.NONE;

        _viewer.css("display", "block");
        _offscreenDiv.css("display", "block");
    }

    function hide() {
        _viewer.css("display", "none");
        _offscreenDiv.css("display", "none");
    }

    function close() {
        if (_timeoutHandle !== null) {
            window.clearTimeout(_timeoutHandle);
            _timeoutHandle = null;
        }

        hide();
    }

    function doNextPanel() {
        if (!_processingNavigationEvents) {
            return;
        }

        if (_pager.isLastPage()) {
            return;
        }
    
        _processingNavigationEvents = false;
        _currentTransitionState = TRANSITION_STATE.NEXT_PAGE;

        updatePageContent(_pager.getNextPage());
    }

    function doPreviousPanel() {
        if (!_processingNavigationEvents) {
            return;
        }

        if (_pager.isFirstPage()) {
            return;
        }

        _processingNavigationEvents = false;

        _currentTransitionState = TRANSITION_STATE.PREVIOUS_PAGE;

        updatePageContent(_pager.getPreviousPage());
    }

    function bindPanelEvents(panel) {
        var $closeButton = panel.find(".imagePanelClose");
        $closeButton.click(function () {
            self.close();
        });

        var $previousButton = panel.find("#imageViewerPrevious").click(doPreviousPanel);
        var $nextButton = panel.find("#imageViewerNext").click(doNextPanel);
    }

    function unbindPanelEvents(panel) {
        panel.find(".imagePanelClose").unbind();
        panel.find("#imageViewerPrevious").unbind()
        panel.find("#imageViewerNext").unbind();
    }

    function loadViewerInitialContent(initialImageId) {
        $.ajax({
            url: "/Galleries/GalleryImageViewer/?id=" + initialImageId,
            type: "get",
            success: function (data, textStatus, jqXHR) {
                _viewer.append(data);

                bindPanelEvents(_viewer);
                buildInitialCache();
                self.updateViewerPanelControls();
                updatePageContent(_pager.getCurrentPage());

                _viewer.css("display", "block");
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var x = 42;
            }
        });
    }

    var self = {
        loadGalleryImages: function (galleryData, selectedImageId) {
            if (!initialized) {
                initialize();
            }

            reset();

            _galleryData = galleryData;
            if (_pager !== null) {
                _pager.destroy();
            }
            _pager = new Pager(_galleryData.getSize(), _galleryData.getIndexForImage(selectedImageId) + 1);
            _pager.registerForEvents(self);

            loadViewerInitialContent(selectedImageId);
        },
        pagesChanged: function (removedPages, addedPages) {
            for (var i = 0; i < removedPages.length; i++) {
                removeCachePage(removedPages[i]);
            }
            for (var i = 0; i < addedPages.length; i++) {
                cachePage(addedPages[i]);
            }
        },
        updateViewerPanelControls: function () {
            var viewerNext = _viewer.find("#imageViewerNext");
          
            if (_pager.isLastPage()) {
                viewerNext.addClass("navButtonDisabled");
            }
            else {
                viewerNext.removeClass("navButtonDisabled");
            }

            var viewerPrevious = _viewer.find("#imageViewerPrevious");

            if (_pager.isFirstPage()) {
                viewerPrevious.addClass("navButtonDisabled");
            }
            else {
                viewerPrevious.removeClass("navButtonDisabled");
            }
        },
        close: function () {
            close();
        },
    };

    return self;
}();