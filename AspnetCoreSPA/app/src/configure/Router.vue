<script>
import Vue from 'vue'
import VueRouter from 'vue-router'
import Customer from '../components/Customer'

Vue.use(VueRouter)

export var router = new VueRouter({
  mode: 'history',
  base: __dirname,
  routes: [
     { path: '/', name: 'customer', component: Customer }


  ]
})
router.beforeEach((to, from, next) =>{
    next();
});
</script>