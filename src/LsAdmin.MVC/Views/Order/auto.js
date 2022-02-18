$(document).on('blur','#budget_price',function(){
		var sum =0;
        var choseLen =0;
        var price_thousand =0;
		var jq = $('.precise_matching input[type=radio]').val();
		var price = $(this).val();
		if(price<100){
		 	$('#budget_price').css('border','1px solid red');
		 	 
            $('#budget_price').attr('placeholder',"最小预算为100");
            return false;
        }else{
            $('#budget_price').css('border','1px solid #b5b5b5');

        }
        if(price>10000000){
		 $('#budget_price').css('border','1px solid red');
		  
            $('#budget_price').attr('placeholder',"最大预算为10000000");
            return false;
        }else{
            $('#budget_price').css('border','1px solid #b5b5b5');

        }
    });
	$(document).on('keyup','#budget_price',function(){
 			var sum =0;
	        var choseLen =0;
	        var price_thousand =0;
			var jq = $('.precise_matching input[type=radio]').val();
		
			if(jq==1){

	 			$('#list_area_2 .check_hy').each(function(i,d){
	 				var _this = $(d);
	 				if(_this.prop('checked')==true){
	 					
		                sum += _this.attr('data-tp') - 0;
		                choseLen++;
				       console.log(sum);
				        if(choseLen>0){
				        	price_thousand = parseInt(sum/choseLen);
				            $('#thousand_price').val(price_thousand);
				            cal_price();
				      
				        }else{
				        	do_expect();
				        	 
				        }
	 				}
	 			})
 			}
 		})
    $(document).on('click','#contact_box .this input[type=button]',function(){
            var price = $('#budget_price').val();
            var ptn = /^\d+(\.\d+)?$/;
            if(!ptn.test(price) ){
                $('#budget_price').css('border','1px solid red');
                $('#budget_price').attr('placeholder',"输入合法的数字，例如：1000");
                return false;
            }else{
                $('#budget_price').css('border','1px solid #b5b5b5');

            }

            var check = $('.precise_matching input[type=radio]:checked').val();

            if(check==1){//精准匹配
                if($('#act-read_num').val()<500){
                    $('#act-read_num').val('');
                    $('#act-read_num').focus();
                    $('#act-read_num').css('border','1px solid red');
                    $('#act-read_num').attr('placeholder','最小阅读量要求：500');
                    return false;
                }else{
                    $('#act-read_num').css('border','1px solid #b5b5b5');
                }

                if($('#act-comment_num').val()<1000){
                    $('#act-comment_num').val('')
                    $('#act-comment_num').focus();
                    $('#act-comment_num').css('border','1px solid red');
                    $('#act-comment_num').attr('placeholder','最小转评赞数要求：1000');
                    return false;
                }else{
                    $('#act-comment_num').css('border','1px solid #b5b5b5');
                }
            }
            
            //提交
            $.ajax({
                cache: true,
                type: "POST",
                url:auto_save_url,
                data:$('#autoForm').serialize(),// 你的formid
                async: false,
                dataType:'json',
                error: function(request) {
                    alert(request);
                },
                success: function(data) {
                     
                    if(data.status==0){
                        //layer.msg(data.msg, {icon: 2,time: 2000});
                        if(data.code==5003){
                            $('#act-code').val('').focus().css('border','1px solid red');
                            $('#act-code').attr('placeholder','请填写正确的验证码');
               
                        }

                        return false;
                    }
                    if(data.status==1){
                        $('#chartId').val(data.id);
                        getChart(data.id)
                        
                        //window.location.href = result_chart_url+"?id="+data.id;
                        categoryStyle();
                    }
                }
            });

            //$('.layui-layer-close1').click();
         
        })
	$(function(){

		$(document).on('click','.point_area',function(){
 
	        $('#industry_box_1').show();
            
	        $('.layui-layer-close1').click();
         
	        $.tools.popupBox(('#industry_box_1'),'地区选择','640px','480px');
            $('#infodorm_one').addClass('this').siblings('.xxk_btn').removeClass('this');
	    })

		$(document).on('click','.point_classi',function(){
           
	        $('#industry_box_2').show();
	         $('.layui-layer-close1').click();
         
	        $.tools.popupBox(('#industry_box_2'),'分类选择','640px','480px');
            $('#infodorm_one').addClass('this').siblings('.xxk_btn').removeClass('this');
	    })
		$(document).on('keyup','#act-read_num',function(){
 
     		$('.act-read').text($(this).val());

     	});
     	$(document).on('blur','#act-read_num',function(){
			var _this = $(this);
			if(_this.val()<500){
        		_this.val('');
        	 
                _this.css('border','1px solid red');
                _this.attr('placeholder','最小阅读量要求：500');
                return false;
            }else{
                $('#act-read_num').css('border','1px solid #b5b5b5');
            }

     		$('.act-read').text($(this).val());

     	});
     	$(document).on('blur','#act-comment_num',function(){
     		var _this = $(this);
     		if(_this.val()<1000){
            	_this.val('')
             
                _this.css('border','1px solid red');
                _this.attr('placeholder','最小转评赞数要求：1000');
                return false;
            }else{
                _this.css('border','1px solid #b5b5b5');
            }
     		$('.act-zan').text($(this).val());
     	});
     	$(document).on('keyup','#act-comment_num',function(){
 
     		$('.act-zan').text($(this).val());
     	});

 		

 		
	 	$('.act-cateCur,.act-provinceCur').click(function () {
            	$('.layui-layer-close1').click();
            })


		/*$('.notes_img').notes_show('.industry_img');
		$('.precise_matching_yes').click(function(){
			$('.precise_match').show();
			$('.search_result ').css({'height':'auto'});
			$('.next_step').addClass('this')
		})
		$('.precise_matching_no').click(function(){
			$('.precise_match').hide();
			$('.search_result ').css({'height':'auto'});
			$('.next_step').addClass('this')
		})*/
		
		
		
 
		$('.point_classi').click(function(){
			$('#industry_box_2').show();
		})
		
		$('#radio_type').click(function(){
            $('#list_area_1 .div_box input[type="checkbox"]').prop('checked',false).attr('disabled',false);
            $('.area_type_box').html('');

        });
        $('#list_area_1 .div_box input[type="checkbox"]').click(function(){
            $('#radio_type').prop('checked',false);
            $(this).attr('disabled',false);
        });
        
        
        $('#list_area_2 .div_box input[type="checkbox"]').click(function(){
            $('#cate_type_t').prop('checked',false);
            $(this).attr('disabled',false);
        });

        $('#cate_type').click(function(){
            $('#list_area_2 .div_box input[type="checkbox"]').prop('checked',false).attr('disabled',false);
            $('.classi_type_box').html('');
        });
        
        $(document).on('click',"#list_area_1 .div_box input[type='checkbox']",function(){
                var _this = $(this);
                var len = $("#list_area_1 .div_box input[type='checkbox']:checked").length;
                var proStr = "";
                var cateStr ="";
                if(len>=3){
                    $("#list_area_1 .div_box input[type='checkbox']").attr('disabled',true);
                    $("#list_area_1 .div_box input[type='checkbox']:checked").each(function(){
                        $(this).attr('disabled',false);
                    })
                   
                }else{
                    $("#list_area_1 .div_box input[type='checkbox']").attr('disabled',false);
                }

                $("#list_area_1 .div_box input[type='checkbox']:checked").each(function(){
                    var oIabel = $(this).next('samp').text();
                    var id = $(this).attr('data-list');
                    proStr += "<span>"+oIabel+"<i class='iconfont_size_10 colse_box' data-list='"+id+"' onclick='colseBox(this)'>&#xe6f7;</i></span>";
                        
                })

                $('.area_type_box').html(proStr);
 
        });
        
        $(document).on('click',"#list_area_2 .div_box input[type='checkbox']",function(){
            var _this = $(this);
            var len = $("#list_area_2 .div_box input[type='checkbox']:checked").length;
            var proStr = "";
            var cateStr ="";
            if(len>=3){
                $("#list_area_2 .div_box input[type='checkbox']").attr('disabled',true);
                $("#list_area_2 .div_box input[type='checkbox']:checked").each(function(){
                    $(this).attr('disabled',false);
                })
            }else{
                $("#list_area_2 .div_box input[type='checkbox']").attr('disabled',false);
            }

            $("#list_area_2 .div_box input[type='checkbox']:checked").each(function(){
                var oIabel = $(this).next('em').text();
                var id = $(this).attr('data-list');
                proStr += "<span>"+oIabel+"<i class='iconfont_size_10 colse_box' data-list='"+id+"' onclick='colseBox(this)'>&#xe6f7;</i></span>";
             
            });

            $('.classi_type_box').html(proStr);
        });
		
	})

	function colseBox(obj){
		var list = $(obj).attr('data-list');
 
		$('.'+list).prop('checked',false);
		$(obj).parent().remove()
	}

    function getChart(id){
        var chartId = $('chartId').val();
        var chartId = $('chartId').val();

        $.ajax({
            cache: true,
            type: "POST",
            url:result_chart_url,
            data: $('#wechatForm').serialize(),// 你的formid
            async: false,
           
            error: function(request) {
                alert(request);
            },
            success: function(data) {

                
                $('#infodorm_one').addClass('this').siblings().removeClass('this');
                $('.condition1,.condition2').css('display','none');
                $('.condition3').css('display','block');
                $('.act-showHide').css('display','block');
                $('.condition3').html(data);
                myChart.setOption(options);
            }
        });

    }

    function getChartData(id){
        $.ajax({
            cache: true,
            type: "POST",
            url:"<?php echo XTools::domainUrl('wechat/wechat-list')?>",
            data:{chartType:1,chartId:id},// 你的formid
            async: false,
            error: function(request) {
                alert(request);
            },
            success: function(data) {
                if(scAreaStyle == "none" && schemdStyle == "none" && scRecommendStyle == "none")
                {  
                    $("#allData").html(data);
                    for (var x=0;x < accountIds.length; x++)
                    {
                        var checkboxName = "user_"+accountIds[x];
                        $(":checkbox[name='"+checkboxName+"']").prop("checked",true);
                        $("#"+checkboxName).addClass("search_result_lier box_h get");
                    } 
            /*      if(p_list!=undefined){
                        if(p_list==1){
                            pg.printHtml(2);
                            pg.printHtml(4);
                        }else{
                            pg.printHtml(2,p_list);
                        pg.printHtml(4,p_list);
                        }
                        
                    }*/
                }
                else if(scAreaStyle == "block")
                {
                    $("#shoucangData").html(data);
                    for (var x=0;x < accountIds.length; x++)
                    {
                        var checkboxName = "sc_user_"+accountIds[x];
                        $(":checkbox[name='"+checkboxName+"']").prop("checked",true);
                        $("#"+checkboxName).addClass("search_result_lier box_h get");
                    }
                }
                else if(schemdStyle == "block")
                {
                    $("#heimingdanData").html(data);
                }
                else
                {
                    $("#tuijianData").html(data);
                    for (var x=0;x < accountIds.length; x++)
                    {
                        var checkboxName = "tj_user_"+accountIds[x];
                        $(":checkbox[name='"+checkboxName+"']").prop("checked",true);
                        $("#"+checkboxName).addClass("search_result_lier box_h get");
                    }
                }
                bindCartEvent('addto_cart');
                
            }
        });
    }
