export const validate = (...rules: validationRule[]) => {
  let validationRules: validationHandler[] = []

  for (let rule of rules){
    validationRules.push(rule())
  }

  return validationRules
}

export const required: validationRule = () => (value: any) => {
  return !!value || `Данное поле обязательно`
}

export const email: validationRule = () => (value: any) => {
  return /.+@.+\..+/.test(value) || `Введите корректный эл.адрес`
}

export type validationHandler = (value: any) => any
export type validationRule = () => validationHandler
