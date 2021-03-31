/*
interface IState {
}

export const state = () => ({
} as IState)

export const getters = {
}

export const mutations = {
}

export const actions = {
}
*/

interface IState {
  orders: any[]
}

export const state = () => ({
  orders: [
    {
      date: "2021-03-19",
      featured: [
        { name: "Фирма", value: "ТестКомпани" },
        { name: "Продукт", value: "XML" },
        { name: "Количество", value: "1 мешок" },
      ],
    },
    {
      date: "2021-03-17",
      featured: [
        { name: "Фирма", value: "ТестКомпани" },
        { name: "Продукт", value: "XML" },
        { name: "Количество", value: "1 мешок" },
      ],
    },
    {
      date: "2021-03-19",
      featured: [
        { name: "Фирма", value: "ТестКомпани" },
        { name: "Продукт", value: "XML" },
        { name: "Количество", value: "1 мешок" },
      ],
    },
    {
      date: "2021-03-04",
      featured: [
        { name: "Фирма", value: "ТестКомпани" },
        { name: "Продукт", value: "XML" },
        { name: "Количество", value: "1 мешок" },
      ],
    },
    {
      date: "2021-03-31",
      featured: [
        { name: "Фирма", value: "ТестКомпани" },
        { name: "Продукт", value: "XML" },
        { name: "Количество", value: "1 мешок" },
      ],
    },
  ]
} as IState)

export const getters = {
  getOrders(state: IState){
    return state.orders;
  }
}

export const mutations = {
}

export const actions = {
}
