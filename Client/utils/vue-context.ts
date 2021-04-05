import { Context } from "@nuxt/types";

let $ctx: Context

export function connectContext(context: Context) {
  $ctx = context
}

export { $ctx }
