<template id="admin-template">
    
    <div class="row">

        <div class="boxfilter">
            <div class="col-md-12">
                <div class="input-group searchbox">
                    <input type="text" class="form-control" v-model="textSearch" placeholder="Search for..." v-on:keyup.enter="onEnterClick">
                </div>
            </div>


        </div>
        <div class="row grid">
             <h3>Contact List</h3>
            <table class="table">
                <thead>
                    <tr>
                        <th v-for="key in columns"  @click="sortBy(key)" :class="sortKey== key ?'active ' + key : key">
                            {{ key | spacewords }}
                            <span v-bind:class="sortKey == key ? (sortOrders[key] > 0 ? 'arrow asc' : ' arrow dsc') : ''">
                    </span>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="entry in listuser" @click="getUserDetails(entry.id)">
                        <td v-for="key in columns">
                            {{entry[key]}}
                        </td>
                    </tr>
                </tbody>
            </table>
            <pagination :current-page="pagination.currentPage" :total-pages="pagination.totalPages" @page-changed="pageChanged">
            </pagination>
        </div>
        <div class="row detail" v-if="!!userdetail">
            <h3>Contact Detail</h3>
            <div class="col-md-6">
                <label class="description">ID</label>
                <label class="value">{{userdetail.id}}</label><br/>
                <label class="description">First Name</label>
                <label class="value">{{userdetail.firstName}}</label>
            </div>
            <div class="col-md-6">
                <label class="description">Email</label>
                <label class="value">{{userdetail.email}}</label><br/>
                <label class="description">Phone1</label>
                <label class="value">{{userdetail.phone}}</label>
            </div>
        </div>
        
    </div>
</template>
<script>
import Vue from 'vue'
import VueResource from 'vue-resource'
import Auth from '../services/Auth'
import Pagination from './Pagination.vue'

Vue.use(VueResource);
export default {
    name: 'Admin',
    template: '#admin-template',
    components : { Pagination},
    data: function () {
        return {
            listuser: null,
            columns: null,
            pagination: {
                currentPage: 1,
                totalPages: 0,
                pageSize:25
            },
            sortKey: null,
            sortOrders: null,
            filterKey:'',
            dataQuery: {
                Text: "",
                CurrentPage: 1,
                NumberItemOnPage:25
            },
            textSearch: "",
            userdetail: null
        }
    },

    mounted: function () {
        this.InitDataGrid('firstName','asc');
    },

    filters: {
    spacewords:function(str)
    {
        var strWordsHeader = '';
        switch(str.toLowerCase()) {
            case 'phone':
                strWordsHeader = 'Phone1';
                break; 
            case 'firstName':
                strWordsHeader = 'First';
                break;   
            case 'lastName':
                strWordsHeader = 'Last';
                break;     
            default:
                strWordsHeader = str.charAt(0).toUpperCase() + str.slice(1)    
        }
        return strWordsHeader;
    }
  },
    methods: {
        sortBy: function (key) {
           this.sortKey = key
           this.sortOrders[key] = this.sortOrders[key] * -1
           this.pagination.currentPage = 1;
            for(var i = 0; i < this.columns.length; i++)
            {
              if(this.columns[i] != key) this.sortOrders[this.columns[i]] = -1;
            }
            this.getListUser();
        },
        InitDataGrid: function(sortKey,sortOrder){
            this.columns =['firstName','lastName','email','phone'];
            var sortOrderColumns = {}
            this.columns.forEach(function (key) {
              sortOrderColumns[key] = -1;
            })
            sortOrderColumns[sortKey] = (sortOrder.toLowerCase() == 'asc')? 1 : -1;
            this.sortOrders = sortOrderColumns;
            this.sortKey= sortKey;
            this.getListUser();
        },
        pageChanged:function (pageNum) {
            this.pagination.currentPage = pageNum;
             this.getListUser();
        },
        onEnterClick(){
            this.getListUser();
        },
        getUserDetails(id){
            var self = this;
            $.ajax({
                url: RestAPIPath.value + '/api/customer/'+id,
                type: "GET",
                headers: {
                    'Access-Control-Allow-Origin': '*'
                },
                crossDomain: true,
                contentType: "application/json; charset=UTF-8",
                success: function(response) {
                    self.userdetail = response.customer;
                   
                },
                error: function(response) {
                    
                }
            }); 
        },
        getListUser: function (){
           
            var dataQuery = {
                TextSearch: this.textSearch,
                CurrentPage: this.pagination.currentPage,
                NumberItemOnPage:this.pagination.pageSize,
                sortKey: this.sortKey,
                orderBy: this.sortOrders[this.sortKey]==1?"asc":"desc"
            }
             var self = this;
            $.ajax({
                url: RestAPIPath.value + '/api/customer/getlistcustomer',
                type: "POST",
                headers: {
                    'Access-Control-Allow-Origin': '*'
                },
                data : JSON.stringify(dataQuery),
                crossDomain: true,
                contentType: "application/json; charset=UTF-8",
                success: function(response) {
                    self.listuser = response.customers;
                    self.pagination.totalPages = Math.ceil(response.total/ self.pagination.pageSize) ;
                },
                error: function(response) {
                    
                }
            });
        }
    }
  }
</script>