var layer = {
    times: new Date().getTime(),
    showCustom: function (_html, _width, _height, _isshadow) {
        var self = this;
        if ($("html").find('.my_layer_sf').length > 0) { console.log(1); return; }
        var css = '<style>' +
            '@keyframes an_ease {from {background:none } to {background-color: rgba(0,0,0,.5); } }' +
            '.my_layer_sf .shadowbg_sf { position: fixed; z-index: 999998; left:0; top:0; width: 100%; height: 100%; background-color: rgba(0,0,0,.5);animation:an_ease ease-in-out .5s; }' +
            '.my_layer_sf .layer_sf { padding:10px; position: fixed; z-index: 999999; display: inline-block; width: ' + _width + 'px; height: ' + _height + 'px; background: #fff; border-radius: 10px; margin-top: -' + (_height / 2) + 'px;margin-right: -' + (_width / 2) + 'px;top: 50%;right: 50%; }' +
            '</style>';
        var html = '<div class="my_layer_sf">' + (_isshadow ? '<div class="shadowbg_sf"></div>' : '') +
            '<div class="layer_sf">' + _html + '</div>' +
            '</div>';
        $("html").append("<div class='layer_sf_" + this.times + "'>" + css + html + "</div>");
        $('.shadowbg_sf').click(function () {
            self.close();
        });
    },
    show: function (config) {
        var self = this;
        if ($("html").find('.my_layer_sf').length > 0) { return; }
        var iconArr = { 'ok': 'ok.png', 'fail': 'fail.png', 'warn': 'warn.png', 'load': 'load.gif' };
        if (config === null) {
            config = { icon: iconArr['ok'], msg: '' };
        } else {
            if (!config.hasOwnProperty('icon')) {
                config.icon = iconArr['ok'];
            } else {
                config.icon = iconArr[config.icon];
            }
            if (!config.hasOwnProperty('msg')) {
                config.msg = "";
            }
        }
        var css = '<style>' +
            '@keyframes an_ease {from {background:none } to {background-color: rgba(0,0,0,.5); } }' +
            '.my_layer_sf .shadowbg_sf { position: fixed; z-index: 999998; left:0; top:0; width: 100%; height: 100%; background-color: rgba(0,0,0,.5);animation:an_ease ease-in-out .5s; }' +
            '.my_layer_sf .layer_sf { position: fixed; z-index: 999999; display: inline-block; width: 150px; height: 150px; background: #fff; border-radius: 10px; margin-top: -75px; top: 50%; margin-right: -75px; right: 50%; }' +
            '.my_layer_sf .layer_sf > div { height: 150px; display: flex; flex-direction: column; justify-content: center; align-items: center; }' +
            '.my_layer_sf .layer_sf > div .waitmsg { color: #999; margin-top: 10px; }' +
            '.my_layer_sf .layer_sf > div img{width:60px;}' +
            '</style>';
        var html = '<div class="my_layer_sf">' +
            '<div class="shadowbg_sf"></div>' +
            '<div class="layer_sf"><div><div><img src="/Resouces/js/layer/' + config.icon + '" /></div><div class="waitmsg">' + (config.msg) + '</div></div></div>' +
            '</div>';
        $("html").append("<div class='layer_sf_" + this.times + "'>" + css + html + "</div>");

        if (config.hasOwnProperty('time')) {
            setTimeout(function () {
                self.close();
                if (config.hasOwnProperty('url')) {
                    location.href = config.url;
                }
            }, config.time * 1000);
        }
    },
    prompt: function (_title, _defaultValue, _fn) {
        var self = this;
        var _html = '<span style="display:block;text-align:right"><a href="javascript:layer.close();" style="display:inline-block;width:18px;height:18px;text-align:center;line-height:16px;background-color:#d1d1d1;color:#fff;border-radius:18px;text-decoration:none" >×</a></span>' +
            '<div style="widit:300px;display:flex;justify-content:space-between;margin-top:20px;">' +
            '<div><input type="text" placeholder="' + _title + '" id="input_prompt" class="text" style="width:200px;padding:10px;border: 1px solid #d1d1d1;box-sizing:border-box;" value="' + _defaultValue + '" /></div>' +
            '<div><input type="button" class="btn" id="submit_prompt" style="width:80px;padding:10px 0;" value="确定"></div>' +
            '</div>';
        self.showCustom(_html, 300, 100, true);
        $('#submit_prompt').click(function () {
            var _newData = $('#input_prompt').val();
            if (_fn) {
                _fn(_newData);
            }
        });
    },
    open: function (_html, _width, _height, _isshadow) {
        this.showCustom(_html, _width, _height, _isshadow);
    },
    load: function () {
        this.show({ icon: 'load', msg: '请稍等...' });
    },
    ok: function (_msg) {
        this.close();
        this.show({ icon: 'ok', msg: _msg, time: 2 });
    },
    ok2: function (_msg, _url) {
        this.close();
        this.show({ icon: 'ok', msg: _msg, time: 2, url: _url });
    },
    success: function (_msg) {
        this.close();
        this.show({ icon: 'ok', msg: _msg, time: 2 });
    },
    fail: function (_msg) {
        this.close();
        this.show({ icon: 'fail', msg: _msg, time: 2 });
    },
    fail2: function (_msg,_url) {
        this.close();
        this.show({ icon: 'fail', msg: _msg, time: 2, url: _url });
    },
    warn: function (_msg) {
        this.close();
        this.show({ icon: 'warn', msg: _msg, time: 2 });
    },
    close: function () {
        $('.layer_sf_' + this.times).remove();
    }
}