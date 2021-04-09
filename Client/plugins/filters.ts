import { Context, Plugin } from '@nuxt/types'
import Vue from 'vue'

let context: Context

const filtersPlugin: Plugin = (ctx: Context) => {
  context = ctx
}

Vue.filter('formatDate', function (value: any) {
  if (typeof value !== "string") return value;
  if (value) {
    return context.$moment(String(value)).format('MMMM Do YYYY')
  }
})

Vue.filter('formatDateTime', function (value: any) {
  if (typeof value !== "string") return value;
  if (value) {
    return context.$moment(String(value)).format('MMMM Do YYYY, h:mm')
  }
})

export default filtersPlugin
