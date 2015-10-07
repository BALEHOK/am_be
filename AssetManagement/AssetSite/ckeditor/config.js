/*
Copyright (c) 2003-2010, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.addStylesSet( 'my_styles',
[
    // Block Styles   
    { name : 'Подзаголовок (h2)' , element : 'h2'},
    { name : 'Абзац (p)' , element : 'p'}
  
]);

    CKEDITOR.editorConfig = function (config) {
        // Define changes to default configuration here. For example:
        config.uiColor = '#AADC6E';
        config.defaultLanguage = 'nl';

        config.toolbar_Basic =
	[
                ['Source', '-', 'Bold', 'Italic', '-', 'NumberedList', 'BulletedList', '-', 'Link', 'Unlink'],
                ['Styles'],
                ['Cut', 'Copy', 'Paste', 'PasteText', 'PasteFromWord'],
                ['Image', 'Flash', 'Table', 'HorizontalRule', 'SpecialChar'],
                ['Maximize', 'ShowBlocks']
	];

        config.toolbar = 'Basic';
        config.resize_enabled = false;
        //config.stylesCombo_stylesSet = 'my_styles';
        //config.contentsCss = '/css/admin.css';
        config.format_p = { element: 'p', attributes: { 'style': ''} };
    };
