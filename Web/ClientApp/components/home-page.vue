<template>
    <div class="container-fluid">
        <div class="row">
            <div class="page-header col-lg-12">  <strong>Home </strong></div>
        </div>
        <div class="row">
            <div class="col-lg-12 box">
                <span>Environment: <strong>{{production?'Production':'Test'}}</strong></span> <span>Current status: <strong> {{running?'Running':'Stopped'}} </strong>  </span>
                <button class="btn btn-primary" v-on:click="stop" v-if="running">{{stopLabel}}</button>
                <button class="btn btn-primary" v-on:click="start" v-if="!running">{{startLabel}}</button>
            </div>
        </div>
        <running-status></running-status>
    </div>
</template>
<script>
    var getData = async function () {
        try {
            let response = await this.$http.get('api/trade');
            this.running = response.data.running;
            this.production = response.data.production;
        } catch (ex) {
            this.running = false;
        }
    };
    export default {
        data() {
            return {
                running: false,
                production: false,
                startLabel: 'Start',
                stopLabel: 'Stop'
            }
        },
        methods: {
            async stop() {
                try {
                    this.stopLabel = 'Stop ...';
                    await this.$http.put('api/trade', { running: false });
                    this.running = false;
                    this.$toastr.s("Stop successfully.");
                } catch (ex) {
                    this.$toastr.e(ex);
                    console.log(ex);
                }
                this.stopLabel = 'Stop';
            },
            async start() {
                try {
                    this.startLabel = 'Start ...';
                    let response = await this.$http.put('api/trade', { running: true });
                    let result = response.data;
                    if (result.isSuccessful) {
                        this.running = true;
                        this.$toastr.s("Start successfully.");
                    } else {
                        this.$toastr.e(result.message);
                    }

                } catch (ex) {
                    this.$toastr.e(ex);
                    console.log(ex);
                }
                this.startLabel = 'Start';
            }
        },
        async created() {
            var self = this;
            getData.call(self);
            this.task = window.setInterval(function () { getData.call(self) }, 2000);
        },
        beforeDestroy() {
            console.log('home. before destory.');
            window.clearInterval(this.task);
        }

    }
</script>
<style>
</style>
