import { Context, Middleware } from "@nuxt/types";
import { commonStore } from "~/store";

const server: Middleware = async (context: Context) => {
  let online = await commonStore?.checkServer()

  if (!online && context.route.path.indexOf("/offline") == -1){
    return context.redirect("/offline")
  }
}

export default server
