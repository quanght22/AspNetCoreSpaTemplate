<template>
  <div id="app">
    <div class="container body-content">
     <router-view></router-view>
    </div>
  </div>
  
</template>
<script>
  import css from './assets/css/Site.scss';
  import Auth from './services/Auth'
  import Vue from 'vue';
  export default {
    name: 'App'
  }
</script>
