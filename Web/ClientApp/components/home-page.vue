<template>
    <div class="container-fluid">
        <div class="row">
            <div class="page-header col-lg-12">  <strong> Running status {{running?'Running':'Stopped'}}</strong></div>
        </div>
        <div class="row">
            <div class="col-lg-12 box">
                <button class="btn btn-primary" v-on:click="stop" v-if="running">Stop</button>
                <button class="btn btn-primary" v-on:click="start" v-if="!running">Start</button>
            </div>
        </div>
        <running-status></running-status>
    </div>
</template>
<script>
    export default {
        data() {
            return {
                running: false
            }
        },
        methods: {
            stop() {
                try {
                    this.$http.put('api/trade', { running: false });
                    this.running = false;
                } catch (ex) {
                    this.$toastr.e(ex);
                    console.log(ex);
                }
            },
            async start() {
                try {
                    let response = await this.$http.put('api/trade', { running: true });
                    let result = response.data;
                    if (result.isSuccessful) {
                        this.running = true;
                    } else {
                        this.$toastr.e(result.message);
                    }
                } catch (ex) {
                    this.$toastr.e(ex);
                    console.log(ex);
                }
            }
        },
        async created() {
            let response = await this.$http.get('api/trade');
            this.running = response.data.running;
        }
    }
</script>
<style>
</style>
