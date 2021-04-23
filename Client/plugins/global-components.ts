import Vue from "vue"

//import
//@ts-ignore
import CustomButton from "~/components/common/CustomButton.vue"

//decraration
const components = {
  CustomButton
}

Object.entries(components).forEach(([name, component]) => {
  Vue.component(name, component)
})
