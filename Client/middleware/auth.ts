import { Middleware, Context } from '@nuxt/types'
import { identityStore } from '~/store'

const adminRoutes = [
  "/orders",
  "/attributes",
  "/types",
  "/templates",
]

const auth: Middleware = async (context: Context) => {

  if (identityStore.jwt == null) {
    await identityStore.initializeIdentity()
  }

  let isAuthenticated = identityStore.isAuthenticated

  if (!isAuthenticated) {
    return context.redirect("/auth/login")
  }

  let currentURL = context.route.path
  let isAdministrator = identityStore.isAdministrator || identityStore.isRoot

  adminRoutes.forEach((adminRoute) => {
    if (currentURL.startsWith(adminRoute) && !isAdministrator) {
      return context.redirect("/")
    }
  })
}

export default auth
