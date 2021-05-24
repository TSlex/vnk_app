import { Context, Plugin } from '@nuxt/types'
import Vue from 'vue'
import { DataType } from '~/models/Enums/DataType'
import { localize } from '~/utils/localizeDataType'

let context: Context

const filtersPlugin: Plugin = (ctx: Context) => {
  context = ctx
}

Vue.filter("textTruncate", function (value: string, maxlength: number = 0) {
  if (typeof value !== "string" || maxlength < 0) return value;

  if (value.length > maxlength){
    return value.substr(0, maxlength) + "...";
  }

  return value;
})

Vue.filter('formatDate', function (value: any) {
  if (value) {
    return context.$moment(String(value)).format('MMMM Do YYYY')
  }
})

Vue.filter('formatDateTime', function (value: any) {
  if (value) {
    return context.$moment(String(value)).format('MMMM Do YYYY, HH:mm')
  }
})

Vue.filter('formatDateTimeUTC', function (value: any) {
  if (value) {
    return context.$moment.utc(String(value)).local().format('MMMM Do YYYY, HH:mm')
  }
})

Vue.filter('formatDataType', function (value: any) {
  if (typeof value !== "string" && isNaN(Number(value))) return value;
  return localize(value as DataType)
})

Vue.filter('formatBoolean', function (value: any) {
  return String(value) === "true" ? "Да" : "Нет";
})

export default filtersPlugin
