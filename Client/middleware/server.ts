import { Context, Middleware } from "@nuxt/types";
import { commonStore } from "~/store";

const server: Middleware = async (context: Context) => {
  let online = await commonStore?.checkServer()

  if (!online){
    return context.redirect("/offline")
  }
}

export default server
