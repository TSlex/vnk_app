export const validate = (...rules: validationRule[]) => {
  let validationRules: validationHandler[] = []

  for (let rule of rules) {
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

export const password: validationRule = () => (value: any) => {
  if (!/.{8,}/.test(value)) {
    return "Пароль должен быть не короче 6 символов"
  }
  if (!/.*[a-z]/.test(value)) {
    return "В пароле должен быть хоть один символ нижнего регистра"
  }
  if (!/.*[A-Z]/.test(value)) {
    return "В пароле должен быть хоть один символ верхнего регистра"
  }
  if (!/.*\d/.test(value)) {
    return "Пароль должен содержать хоть одно число"
  }
  if (!/.*[#$^+=!*()@%&\-_]/.test(value)) {
    return "Пароль должен содержать хоть один спец.символ"
  }

  return true
}

export const minlength: validationRuleWithParam = (length: number) => (value: any) => {
  return value.length >= length || `Поле должно быть не короче ${length} символов`
}

export const maxlength: validationRuleWithParam = (length: number) => (value: any) => {
  return value.length <= length || `Поле должно быть не длиннее ${length} символов`
}

export type validationHandler = (value: any) => any
export type validationRule = () => validationHandler
export type validationRuleWithParam = (param: any) => validationHandler
