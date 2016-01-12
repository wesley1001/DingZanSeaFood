$(document).ready(function(){
	$(".f-menu").stop(true,false).animate({"left":"0"},500);                     
	/*地区选择*/
	$("#head .local em").click(function(){
		$(".show-hidden").stop(true,false).slideDown("fast");
	});
	$(".local-select ul li a , .guidebox  ul li a ").click(function(){
		$("#head .local em").text($(this).text());
		$(".show-hidden").stop(true,false).slideUp("fast");
		
		/* 后台设置区域*/
		var url		='/index/setCity';
		var code	= $(this).attr('seq');
		$.post(url,{'code':code},function(){
			window.location.href=self.location
		},'json');
		
	});
	/*地址选择*/
	$("#contents .adr-list .area").click(function(){
		var v = $(this).attr("data-value");
			window.location.href = "/Product/xiadan?d="+v;
	});

	$('#csubmit').click(function(){

		if($('#vipaddress2').val() == '')
        {
    		jQuery.alertWindow("错误提示","留下姓名哦～");
            return false;
        }
        if($('#vipaddress3').val() == '')
        {
    		jQuery.alertWindow("错误提示","留下电话号码哦～");
            return false;
        }
        else
        {
            var patrn = /^1[3|4|5|7|8][0-9]\d{8}$/;
            if(patrn.exec($('#vipaddress3').val())){
                errorprompt('');
            }
            else
            {
        		jQuery.alertWindow("错误提示","电话格式不正确哦～");
                return false;
            }
        }

		var address = $('#vipaddress1').find("option:selected").attr('address');
		var lat = $('#vipaddress1').find("option:selected").attr('lat');
		var lng = $('#vipaddress1').find("option:selected").attr('lng');
		var bz = $('#bz').val();
		var data = {
			address:address,
			lat:lat,
			lng:lng,
			bz:bz,
			name : $('#vipaddress2').val(),
			phone : $('#vipaddress3').val(),
			combo_date : $('#combo_date').val(),
			combo_time : $('#combo_time').val(),
		};

		$.ajax({
	        type:"post",
	        url: "/Index/Cchangeaddr",
	        data:data,
	        dataType: "html",
	        success: function(res){
	        	// console.log(res);
	        	if(res == "0"){
	        		window.location.href = "/wxpay/pay?orderid=" + $('#csubmit').attr("data-value");
	        	}
	        	if(res == "1"){
	        		jQuery.alertWindow("错误提示","您的购物车里没有商品哦！");
	        	}
	        	if(res == "2"){
	        		jQuery.alertWindow("错误提示","您还没有填写姓名或者电话哦！");
	        	}
	        	if(res == "3"){
	        		jQuery.alertWindow("错误提示","您的优惠券不存在或已被使用！");
	        	}
	        	if(res == "4"){
	        		jQuery.alertWindow("错误提示","服务器繁忙！");
	        	}
	        	if(res == "7"){
	        		jQuery.alertWindow("错误提示","18:30后就不能订购明天的沙拉了哦~");
	        	}
	        }
     	});
	});
	//提交订单
	$('#nosubmit').click(function(){
		jQuery.alertWindow("错误提示","亲，单独订购面包不支持配送哦，还是搭配我们健康又美味的沙拉一起吧~");
	});

	$('#dosubmit').click(function(){
        // var goodslist = $('#goodslist li');
        // if(goodslist.length==1) {
        //     var goods = goodslist.first();
        //     if(goods.attr('id')=='g4' && goods.attr('data')=='t0'){
        //         jQuery.alertWindow("错误提示","亲，单独订购面包不支持配送哦，还是搭配我们健康又美味的沙拉一起吧~");
        //         return false;
        //     }
        // }
		// var shiptime = $('#time').val();
		var bz = $("#bz").val();
		// if(shiptime == 0)
		// {
	 //        jQuery.alertWindow("错误提示","请选择配送时间");
	 //        return false;
		// }
		// var shipdate = $('#date').val();
		var dizhiid = $("#dizhiid").val();
		if(dizhiid == ""){
			jQuery.alertWindow("错误提示","请选择配送地址");
			return false;
		}
		var orderid = $(this).attr("data-value");
		var result = false;
		var yhjtype = $("#selcoupon").val();
		var total = $("#total_money").html();
        if(total*1 < 25 && yhjtype > 0){
        	jQuery.alertWindow("错误提示","订单满25元才可使用代金券！");
			return false;
        }
		if(yhjtype == -1){
			var yhj = $("#yhj").val();
			if(yhj == "") {
                jQuery.alertWindow("错误提示", "请输入优惠券！");
                return false;
            }else if(total*1 < 25){
                jQuery.alertWindow("错误提示","订单满25元才可使用代金券！");
                return false;
            }else{
				$.ajax({
					type:"post",
		            url: "/Index/checkcoupon",
		            data:{yhj:yhj,orderid:orderid},
		            dataType: "html",
		            success: function(res){
		            	if(res == "0"){
		            		jQuery.alertWindow("错误提示","您的优惠券不存在或已被使用！");
		            		result = true;
		            	}else{
		            		newbuy(orderid,dizhiid,bz);
		            	}
		            }
	          	});
			}
		}else{
			newbuy(orderid,dizhiid,bz);
		}
		/*$.ajax({
            type:"post",
            url: "/Index/ajaxsubmit",
            data:{payPrice:price,deliveryDate:shipdate,deliveryTime:shiptime,pid:product_id,addrid:addr_id,jiangzhi:jiangzhi},
            dataType: "html",
            success: function(res){
              	jQuery.alertWindow("成功提示",res,"","/Index/nopay");
            }
          });*/

	});
	$("#baomingsave").click(function(){
		//var openid = $(this).attr("data-value");
		var ly = $("#ly").val();
		var num = $("#num").val();
		var hym = $("#hym").val();
		var company = $("#company").val();
		var addr = $("#addr").val();
		var mobile = $("#mobile").val();
		if(ly == ''){
			alert('请填写申请理由');
			return false;
		}
		if(num == ''){
			alert('请填写参与人数');
			return false;
		}
		if(hym == ''){
			alert('请填写会员名');
			return false;
		}
		if(company == ''){
			alert('请填写公司名称');
			return false;
		}
		if(addr == ''){
			alert('请填写办公室地址');
			return false;
		}
		if(mobile == ''){
			alert('请填写联系电话');
			return false;
		}
		$.ajax({
            type:"post",
            url: "/Index/hdsubmit",
            data:{ly:ly,num:num,hym:hym,company:company,addr:addr,mobile:mobile},
            dataType: "html",
            success: function(res){
            	jQuery.alertWindow("温馨提示",res,"","/Index/index");
            }
          });
	});
	$(".huodaopay").click(function(){
		var openid = $(this).attr("data-value");
		var orderid = $(this).attr("data-order");
		$.ajax({
            type:"post",
            url: "/Index/ajaxsubmit",
            data:{openid:openid,orderid:orderid},
            dataType: "html",
            success: function(res){
            	jQuery.alertWindow("温馨提示",res,"","/Product/allOrder");
            }
          });
	});
	$("#save").click(function(){
		var openid = $(this).attr("data-value");
		var orderid = $(this).attr("data-order");
		$.ajax({
            type:"post",
            url: "/Index/ajaxsubmit",
            data:{openid:openid,orderid:orderid},
            dataType: "html",
            success: function(res){
            	jQuery.alertWindow("温馨提示",res,"","/Product/allOrder");
            }
          });
	});
	$('#paiduisubmit').click(function(){
            window.location.href = "http://wx.tianxinfood.com.cn/wxpay/pay?type=pd";
    });
	/*蒙板展示*/
	var city_code = $.cookie('city_code');
	if(typeof(city_code) == 'undefined' ||city_code == '')
	{
		$('.g-mask').show();
		$('.guidebox').show();
		$(".f-menu").css({"left":"0"});
	}
	
	/*记录用户来源cookie*/
	var id = request('id');
	var cookietime = new Date(); 
    cookietime.setTime(cookietime.getTime() + (30 * 24 * 60 * 60 * 1000));
    $.cookie('source',id,{expires:cookietime, path: '/'});
    
	/*详情页TAB选择*/
	$(".tabs li").each(function(index){
		$(".tabs li").eq(index).click(function(){
			$(".tabs li").removeClass("active");
			$(this).addClass("active");
			$(".tabbox").removeClass("active");
			$(".tabbox").eq(index).addClass("active");
		});
	});
	/*购物车选区*/
	$(".cart_list li .area").each(function(index){
		$(".cart_list li .area").eq(index).click(function(){
			if($(this).find(".check").hasClass("on"))
			{
				$(this).find(".check").removeClass("on");
			}
			else
			{
				$(this).find(".check").addClass("on");
			}
		});
	});
	
	/*餐具*/
	$(".birthday .link-style").click(function(){
		if($(this).find(".btn-onoff").hasClass("active"))
		{
			$(this).find(".btn-onoff").removeClass("active");
			$("#canju").val('');
		}
		else
		{
			$(this).find(".btn-onoff").addClass("active");
			$("#canju").val('1');
		}
	});
	/*详情页朋友圈分享蒙版*/
	$(".mask-a, .text-point").click(function(){
		$("#wrap").removeAttr("style");
		$(".mask-a, .text-point").hide();
	});
	/*优惠券*/
	$(function () {
	    $("#selcoupon").change(function(){
	        var v = $(this).val();
	        var total = $("#total_money").html();
	        if(total*1 < 25){
	        	$("#selcoupon option").eq(0).attr("selected",true);
	        }else{
		        if(v == -1){
		        	$("#yhj").css("display","block");
		        }else{
		        	$("#yhj").css("display","none");
			        var orderid = $(this).attr("data-order");
			        var openid = $(this).attr("data-value");
			        $.ajax({
			            type:"post",
			            url: "/Index/setcoupon",
			            data:{v:v,orderid:orderid,openid:openid},
			            dataType: "json",
			            success: function(res){
			            	if(res.error == 0){
			            		//window.location.href = window.location.href;
			            	}
			            }
			        });
		        }
	        }
	    });
	    $("#yhj").blur(function(){
	    	var v = $(this).val();
	    	var orderid = $("#selcoupon").attr("data-order");
	        var openid = $("#selcoupon").attr("data-value");
	    	if(v == ""){
	    		return false;
	    	}else{
		    	$.ajax({
		            type:"post",
		            url: "/Index/setothercoupon",
		            data:{v:v,orderid:orderid,openid:openid},
		            dataType: "json",
		            success: function(res){
		            	if(res.error == 0){
		            		//window.location.href = window.location.href;
		            	}
		            }
		        });
		    }
	    });
	});

});
function newbuy(orderid,dizhiid,bz){
	$.ajax({
        type:"post",
        url: "/Index/changeaddr",
        data:{orderid:orderid,addrid:dizhiid,bz:bz,combo_date:$('#combo_date').val(),combo_time:$('#combo_time').val()},
        dataType: "html",
        success: function(res){
        	if(res == "1"){
        		jQuery.alertWindow("错误提示","您的购物车里没有商品哦！");
        	}
        	if(res == "2"){
        		jQuery.alertWindow("错误提示","您还没有选择地址哦");
        	}
        	if(res == "3"){
        		jQuery.alertWindow("错误提示","您的优惠券不存在或已被使用！");
        	}
        	if(res == "4"){
        		jQuery.alertWindow("错误提示","服务器繁忙！");
        	}
        	if(res == "0"){
        		window.location.href = "/wxpay/pay?orderid="+orderid; 
        	}
        	if(res == "7"){
        		jQuery.alertWindow("错误提示","18:30后就不能订购明天的沙拉了哦~");
        	}
        }
      });
}

/*详情页数量加减*/
function MinusPlus(a)
{
	var n = parseInt($(".count .num").val());
	if(a=='m')
	{
		if(n>1)
		{
			n--;
		}
	}
	else
	{
		n++;
	}
	$(".count .num").val(n)
}

/*详情页朋友圈分享蒙版*/
function showPoint()
{
	$(".mask-a, .text-point").show();
}

/*光标定位*/
function focusCursor(obj){
	var input = obj;
	setTimeout(function() {	// chrome下要加settimeout
		var n = input.value.length;
		input.setSelectionRange(n, n);
		input.focus();
	}, 10);
}

var weixinApp  = {}; //微信公共对象

weixinApp.inputFocus = function(){ //页面input框的foucs和blur的切换效果  **注意用这个效果页面的input框都是输入框
	$(".input").each(function(index){
		$(".input").eq(index).focus(function(){
			$(this).addClass("fc");
			if($(this).val().length>0)
			{
				$('.removetext').eq(index).show();
			}
		});
		$(".input").eq(index).blur(function(){
			$(this).removeClass("fc");
			setTimeout(function(){ $('.removetext').eq(index).hide()},200);
		});
	});
};

weixinApp.inputKeydown = function(){ //按输入框就会有关闭的提示
	function keydown(){
		$(".input").each(function(index){
			var $input = $(this);
			if($input.hasClass("fc"))
			{
				if($input.val()=="")
				{
					$('.removetext').eq(index).hide();
				}
				else
				{
					$('.removetext').eq(index).show();
				}
			}
		});	
	}
	if(/msie/i.test(navigator.userAgent))  //ie浏览器  
	{
		document.onpropertychange=keydown; 
	} 
	else  //非ie浏览器，比如Firefox 
	{
		document.addEventListener("input",keydown,false); 
	}

};

	


weixinApp.ios5fixed_user = function(){
	if(navigator.userAgent.match(/iPhone|iPad/i) != null){
		var iOSKeyboardFix = {
			  targetElem: $('.f-menu'), //浮动
			  init: (function(){
				$("input, textarea").bind("focus", function() {
				  iOSKeyboardFix.bind();
				});
			  })(),

			  bind: function(){
					$(document).bind('scroll', iOSKeyboardFix.react);  
					iOSKeyboardFix.react();      
			  },

			  react: function(){ //响应
					  var offsetX  = iOSKeyboardFix.targetElem.offset().top;
					  var scrollX = $(window).scrollTop();
					  var changeX = offsetX - scrollX; 
					  iOSKeyboardFix.targetElem.css({'position': 'fixed', 'top' : '-'+changeX+'px'});
					  $('input, textarea').bind('blur', iOSKeyboardFix.undo);
					  $(document).on('touchstart', iOSKeyboardFix.undo);
			  },

			  undo: function(){
				  iOSKeyboardFix.targetElem.removeAttr('style');
				  document.activeElement.blur();
				  $(document).unbind('scroll',iOSKeyboardFix.react);
				  $(document).unbind('touchstart', iOSKeyboardFix.undo);
				  $('input, textarea').unbind('blur', iOSKeyboardFix.undo);
			  }
		};
	}
}

