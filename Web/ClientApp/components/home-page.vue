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
                running: false,
                errorMessage,
            }
        },
        methods: {
            stop() {
                this.running = false;
                this.$http.put('api/trade', { running: false });

            },
            async start() {
                let response = this.$http.put('api/trade', { running: true });
                if (response.data.isSuccessful) {
                    this.running = true;
                } else {
                    alert(response.data.message);
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
