import { Context, Plugin } from '@nuxt/types'
import Vue from 'vue'
import { DataType } from '~/models/Enums/DataType'
import { localize } from '~/utils/localizeDataType'

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

Vue.filter('formatDataType', function (value: any) {
  if (typeof value !== "string" && isNaN(Number(value))) return value;
  return localize(value as DataType)
})

Vue.filter('formatBoolean', function (value: any) {
  return value === "true" ? "Да" : "Нет";
})

export default filtersPlugin
