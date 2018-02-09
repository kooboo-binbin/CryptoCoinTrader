import Vue from 'vue'
import axios from 'axios'
import router from './router'
import store from './store'
import Toastr from 'vue-toastr';
import { sync } from 'vuex-router-sync'
import App from 'components/app-root'

Vue.use(Toastr);
Vue.prototype.$http = axios;

sync(store, router)

const app = new Vue({
    store,
    router,
    ...App
})

export {
    app,
    router,
    store
}
