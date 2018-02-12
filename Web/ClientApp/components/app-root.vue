<template>
    <div id="app" class="container-fluid">
        <div class="row">
            <div class="col-sm-2">
                <nav-menu params="route: route"></nav-menu>
            </div>
            <div class="col-sm-10 bg-gray">
                <router-view></router-view>
            </div>
        </div>
    </div>

</template>
<script>
    import Vue from 'vue'
    import CounterExample from './counter-example'
    import HomePage from './home-page'
    import NavMenu from './nav-menu'
    import Pagination from './pagination.vue'
    import RunningStatus from './running-status.vue'

    Vue.component('counter-example', CounterExample);
    Vue.component('home-page', HomePage);
    Vue.component('nav-menu', NavMenu);
    Vue.component('pagination', Pagination);
    Vue.component('running-status', RunningStatus);

    export default {
        data() {
            return {
            }
        }
    }
</script>
<style>
</style>
