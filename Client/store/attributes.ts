interface IState {
  attributes: any[]
}

export const state = () => ({
  attributes: [
    { name: "Время погрузки", value: "8:00" },
    { name: "Название фирмы", value: "BlablaCompany LTD" },
    { name: "Место доставки", value: "Marselle, France" },
    { name: "Продукт", value: "HCR-105" },
    { name: "Упаковка", value: "500 kg bags" },
    { name: "Номер заказа", value: "134561363452345" },
    { name: "Перевозчик", value: "NOVOTRADE LOGISTICS" },
  ],
} as IState)

export const getters = {
  getAttributes(state: IState){

    return state.attributes;
  }
}

export const mutations = {
}

export const actions = {
}
