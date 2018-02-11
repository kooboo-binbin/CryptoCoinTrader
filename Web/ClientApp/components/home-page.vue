<template>
    <div>
        <h1> Running status {{running?'Running':'Stopped'}}</h1>
        <button class="btn btn-default" v-on:click="stop" v-if="running">Stop</button>
        <button class="btn btn-default" v-on:click="start" v-if="!running">Start</button>

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
