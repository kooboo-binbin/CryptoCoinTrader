import './css/site.css'
import 'core-js/es6/promise'
import 'core-js/es6/array'
import 'bootstrap'
import { app } from './app'
import message from './message.js'

app.$mount('#app')
message.start(app);

