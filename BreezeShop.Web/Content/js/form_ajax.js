function trim(str){   
    str = str.replace(/^(\s|\u00A0)+/,'');   
    for(var i=str.length-1; i>=0; i--){   
        if(/\S/.test(str.charAt(i))){   
        str = str.substring(0, i+1);   
        break;   
        }   
    }   
    return str;   
}  
function phoneNumber(phone){
    var patrn = /^1[3|4|5|8][0-9]\d{8}$/;
    if(patrn.exec(phone)){
        errorprompt(" ");
        return true;
    }else{  
        errorprompt('手机号格式不正确');
        return false;
    }
}
function userName(name){
    if(name == ''){
        errorprompt('用户名不能为空');
        return false;
    }
    var patrn = /^([\u4E00-\uFA29]|[\uE7C7-\uE7F3]|[a-zA-Z0-9])*$/;
    if(patrn.exec(name)){
        errorprompt(" ");
        return true;
    }else{
        errorprompt('用户名不能含有特殊字符');
        return false;
    }
}
function regexPassword(password){
    if(password.length < 6 || password.length > 20)
    {
        errorprompt('密码长度为6-14位');
        return false;
    }
    var patrn = /([a-zA-Z]*[<>-`=\\\[\];',./~!@#$%^&*()_\+|{}:"?]*\d*)$/;
    if(patrn.exec(password)){
        return true;
    }else{
        errorprompt('密码不能有特殊字符');
        return false;
    }

}
function checkMail(mail){
    var patrn = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.(com|cn|com.cn|net|net.cn|org|org.cn|gov.cn|biz|info|asia|mobi|hk|tv|tel|cc|me|sina.com){1}$/;
    if(patrn.exec(mail)){
        errorprompt(" ");
        return true;
    }else{
        errorprompt('邮箱格式不正确');
        return false;
    }
}
function errorprompt(txt){
    $('#error').text(txt);
	if($('#error').text()=='')
	{
		$('#error').addClass('bgnone');
	}
	else
	{
		$('#error').removeClass('bgnone');
	}
}
function ajax_connect(data,url){
    if(data == '' || url == '') return false;
    $.post(url, data,function(data){
           if(data.err == 1){
        	   errorprompt(data.msg);
           }
        },'json' );
}
function request(paras)
{ 
    var reg = new RegExp("(^|&)"+ paras +"=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg);  //匹配目标参数
    if (r!=null) return unescape(r[2]); return null; //返回参数值
}
function verification(data,url){
	$.ajaxSetup({
		async: false
	}); 
    if(data == '' || url == '') return false;
    var res = $.post(url, data,function(data){
        return data;
    },'json' ).responseText;
    return res;
}

function GetRequest() {
   var url = location.search; //获取url中"?"符后的字串
   var theRequest = new Object();
   if (url.indexOf("?") != -1) {
      var str = url.substr(1);
      strs = str.split("&");
      for(var i = 0; i < strs.length; i ++) {
         theRequest[strs[i].split("=")[0]]=unescape(strs[i].split("=")[1]);
      }
   }
   return theRequest;
}

function isWeiChat(){
    var ua = navigator.userAgent.toLowerCase();
    if(ua.match(/MicroMessenger/i)=="micromessenger") {
        return true;
    } else {
        return false;
    }
}

function setCookie(name,value,t)
{
    var exp = new Date();
    exp.setTime(exp.getTime() + t);
    document.cookie = name + "="+ escape (value) + ";expires=" + exp.toGMTString();
}

//读取cookies
function getCookie(name)
{
    var arr,reg=new RegExp("(^| )"+name+"=([^;]*)(;|$)");
    if(arr=document.cookie.match(reg))
        return (arr[2]);
    else
        return null;
}

//删除cookies
function delCookie(name)
{
    var exp = new Date();
    exp.setTime(exp.getTime() - 10000);
    var cval=getCookie(name);
    if(cval!=null)
        document.cookie= name + "="+cval+";expires="+exp.toGMTString()+";path=/;secure";
}

function showload(){
    $(".loadmask").show();
}
function closeload(){
    $(".loadmask").hide();
}