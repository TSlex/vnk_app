export interface OrderGetDTO {
  id: number;
  name: string;
  completed: boolean;
  overdued: boolean;
  notation: string;
  executionDateTime: Date;
  attributes: OrderAttributeGetDTO[];
}

export interface OrderAttributeGetDTO {
  id: number;
  name: string;
  usesDefinedValues: boolean;
  usesDefinedUnits: boolean;
  customValue: string;
  valueId: number;
  unitId: number;
  attributeId: number;
  typeId: number;
  type: string;
  dataType: number;
  featured: boolean;
}

export interface OrderPostDTO {
  name: string;
  completed: boolean;
  notation: string;
  executionDateTime: Date;
  attributes: OrderAttributePostDTO[];
}

export interface OrderAttributePostDTO {
  featured: boolean;
  attributeId: number;
  customValue: string;
  valueId: number;
  unitId: number;
}

export interface OrderCompletionPatchDTO {
  id: number;
  completed: boolean;
}

export interface OrderPatchDTO {
  id: number;
  name: string;
  completed: boolean;
  notation: string;
  executionDateTime: Date;
  attributes: OrderAttributePatchDTO[];
}

export interface OrderAttributePatchDTO {
  id: number;
  patchOption: PatchOption;
  featured: boolean;
  attributeId: number;
  customValue: string;
  valueId: number;
  unitId: number;
}

export interface OrderHistoryDTO {
  id: number;
  masterId: number;
  createdBy: string;
  createdAt: Date;
  changedBy: string;
  changedAt: Date;
  deletedBy: string;
  deletedAt: Date;
  name: string;
  completed: boolean;
  overdued: boolean;
  notation: string;
  executionDateTime: Date;
  attributes: OrderAttributeHistoryDTO[];
}

export interface OrderAttributeHistoryDTO {
  id: number;
  name: string;
  usesDefinedValues: boolean;
  usesDefinedUnits: boolean;
  customValue: string;
  valueId: number;
  unitId: number;
  attributeId: number;
  typeId: number;
  type: string;
  dataType: number;
  featured: boolean;
}
