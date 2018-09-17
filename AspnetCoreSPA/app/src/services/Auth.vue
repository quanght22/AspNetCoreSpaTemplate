<script>
import Vue from 'vue'
import {router} from '../configure/Router'
const API_URL = RestAPIPath.value;
const SIGNUP_URL = API_URL + '/Account/Register'
const UPDATE_URL = API_URL + '/Account/updateuser'
const LOGIN_URL = API_URL + '/connect/token'
export default {
    name: 'Auth',
    user: {
      authenticated: false,
      name:'',
      fullAddress:'',
      roleName:''
    },
    login(context, creds, redirect) {
       var data = JSON.stringify(creds.body);
        data = JSON.parse(data);
        this.authentication(context, data, redirect);
    },
    authentication(context, data, redirect) {
      context.start();
      var self = this;
     $.ajax({
        url: LOGIN_URL,
        type: "POST",
         crossDomain: true,
        contentType: "application/x-www-form-urlencoded; charset=UTF-8", // send as JSON
        data:{
            grant_type: 'password',
            username: data.emailaddress,
            password: data.password,
            scope: 'openid+email+name+profile+roles',
        },
        success: function(response) {
            context.finish();
            localStorage.setItem('expires_in',response.expires_in);
            localStorage.setItem('auth-tokens-expiration',self.getExpirationDate(response.expires_in));
            localStorage.setItem('auth-tokens',response.access_token)
            self.getRoleName(self).then((data) => {
                self.user.roleName = data[0];
                localStorage.setItem('user-role', self.user.roleName);
                if(self.user.roleName == "Administrator"){
                    context.GoAdminPage();
                }
                else context.GoHomePage();
            });
            self.getUserInfor()
        },
        error: function(response) {
           context.fail();
           context.error = response.responseJSON.error_description;
        }
     });

    },
    getRoleName: function(object){
        var self = this;
           return new Promise((resolve, reject) => {
                var request = $.ajax({
                    url: RestAPIPath.value + '/account/userrole',
                    type: "GET",
                    contentType: "application/json; charset=UTF-8", // send as JSON
                    async: false,
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader('Authorization', self.getToken());
                    }
                });
                request.then(response => resolve(response)).catch(() => reject)
            });
    },
    getExpirationDate(expires_in){
      var now = new Date();
      return new Date(now.getTime() + expires_in * 1000).getTime();
    },
    signup(context, creds, redirect) {
        context.start();
        var self = this;
        var data = JSON.stringify(creds);
        $.ajax({
            url: SIGNUP_URL,
            type: "POST",
            contentType: "application/json; charset=UTF-8",
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', self.getToken());
            },
            data:data,
            success: function(response) {
                context.finish();
                context.showWarningModal =true;
            },
            error: function(response) {
                context.finish();
                if(response.status == 401) context.GoLogin();
                else{
                    var index = 0;
                    var messages = '';
                    var errors = response.responseJSON;
                    while(typeof errors[index] != "undefined")
                    {
                        if(messages !='') messages += '<br/>';
                        messages += errors[index][0];
                        index++;
                    }
                    context.errorMessage = messages;  
                }
            }
        });
    },
    update(context, creds) {
        context.start();
        var self = this;
        var data = JSON.stringify(creds);
        $.ajax({
            url: UPDATE_URL,
            type: "POST",
            contentType: "application/json; charset=UTF-8",
            beforeSend: function (xhr) {
                xhr.setRequestHeader('Authorization', self.getToken());
            },
            data:data,
            success: function(response) {
                context.finish();
                context.showDealerEditModal = false;
                context.$parent.showDealerEditModal = false;
                context.$parent.ReloadData();
            },
            error: function(response) {
                context.fail();
                if(response.status == 401) context.GoLogin();
                else  context.errorMessage = response.responseJSON.error_description
            }
        });

    },

    logout(norefresh) {
      localStorage.removeItem('auth-tokens')
      localStorage.removeItem('expires_in');
      localStorage.removeItem('auth-tokens-expiration')
      localStorage.removeItem('user-role')
      localStorage.removeItem('user-fulladdress')
      localStorage.removeItem('user-name')
      this.user.authenticated = false
      if(norefresh !=true) window.location.href = '/';
    },

    checkAuth() {
      var tokens_expiration = JSON.parse(localStorage.getItem('auth-tokens-expiration'));
      if (tokens_expiration > new Date().getTime()) {
        localStorage.setItem('auth-tokens-expiration',this.getExpirationDate(localStorage.getItem('expires_in')));
        this.user.authenticated = true;
        this.user.roleName = localStorage.getItem('user-role');
        this.user.name = localStorage.getItem('user-name');
        this.user.fullAddress = localStorage.getItem('user-fulladdress');
        return true;
      }
      this.user.authenticated = false
      return false;
    },
    getAuthHeader() {
      var authTokens = localStorage.getItem('auth-tokens');
      if(authTokens) {
        return {
          'Authorization': 'Bearer ' + authTokens
        }
      }
      return null;
      
    },
    getToken() {
      var authTokens = localStorage.getItem('auth-tokens');
      if(authTokens) {
        return 'Bearer ' + authTokens;
      }
      return null;
    },
    getUserInfor: function (){
        var self = this;
        $.ajax({
        url: RestAPIPath.value + '/account/userinfor',
        type: "GET",
        contentType: "application/json; charset=UTF-8", // send as JSON
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', self.getToken());
        },
        success: function(response) {
                if(response.name !=null){
                      self.user.name = response.name;
                      localStorage.setItem('user-name', self.user.name);
                }
                 if(response.fullAddress !=null){
                       self.user.fullAddress = response.fullAddress;
                       localStorage.setItem('user-fulladdress', self.user.fullAddress); 
                 }
            },
        error: function(response) {
        }
        });
    }
}
</script>
