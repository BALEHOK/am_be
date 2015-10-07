(function ($) {
    var bubbledEvents = ['click', 'dblclick', 'mousedown', 'mouseup', 'mousemove', 
        'mouseover', 'mouseout', 'keydown', 'keypress', 'keyup'];
    var allowedEvents = { };
    $.each(bubbledEvents, function(idx,event) { allowedEvents[event] = true; });
    $.fn.extend({
        delegate: function (event, selector, func) {
            return $(this).each(function () {
                if (allowedEvents[event])
                    $(this).bind(event, function (e) {
                        var el = $(e.target), result = false;
                        while (!$(el).is("body")) {
                            if ($(el).is(selector)) {
                                result = func.apply($(el)[0], [e]);
                                if (result === false) { e.preventDefault(); }
                                return;
                            }
                            el = $(el).parent();
                        }
                    });
            });
        },
      
  undelegate: function (event) { return $(this).each(function () { 
            $(this).unbind(event); }); }
    });
})(jQuery);