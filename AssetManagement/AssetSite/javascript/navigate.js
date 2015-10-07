$(document).keydown(function(e) {
    switch (e.keyCode ? e.keyCode : e.which ? e.which : null) {
        case 0x25:            
            $('.btnPrev').click();
            break;
        case 0x27:     
            $('.btnNext').click();
            break;
    }    
});